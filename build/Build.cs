﻿using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
//[GitHubActions(
//    "Build",
//        GitHubActionsImage.MacOsLatest,
//        GitHubActionsImage.UbuntuLatest,
//        GitHubActionsImage.WindowsLatest,
//        On = new []
//        {
//            GitHubActionsTrigger.Push,
//            GitHubActionsTrigger.PullRequest
//        },
//        AutoGenerate = true,
//        PublishArtifacts = true,
//        InvokedTargets = new[] { nameof(Full) })]
partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Dev);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    bool PublishRelease => Equals(Configuration, Configuration.Release);

    [Parameter(".NET Runtime ID")]
    readonly string Runtime = "win-x64";

    [Solution("rmbox.sln")] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework="net5.0")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath PluginsDirectory => RootDirectory / "plugins";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath DistDirectory => RootDirectory / "dist";
    AbsolutePath PackDirectory => DistDirectory / "RuminoidToolbox";
    AbsolutePath OutputDirectory => PackDirectory / "rmbox";
    AbsolutePath ToolsDirectory => OutputDirectory / "tools";

    readonly string[] ExcludeSourceFolders =
    {
        "rmbox-vpygen",
        "rmbenv"
    };

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            PluginsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);

            DeleteDirectory(DistDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target PrepareBuildProj => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            new[]
                {
                    SourceDirectory / "rmbox/rmbox.csproj",
                    SourceDirectory / "rmbox-shell/rmbox-shell.csproj"
                }
                .ForEach(x =>
                    File.WriteAllText(
                        x,
                        File.ReadAllText(x)
                            .Replace(
                                "<!-- RMBOX_BUILD_PROPS -->",
                                $"<RuntimeIdentifier>{Runtime}</RuntimeIdentifier><SelfContained>true</SelfContained>")));
        });

    Target Publish => _ => _
        //.DependsOn(Compile)
        .DependsOn(PrepareBuildProj)
        .Executes(() =>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed

            new[]
                {
                    SourceDirectory,
                    PluginsDirectory
                }
                .SelectMany(x =>
                    Directory.EnumerateDirectories(x))
                .SelectMany(x =>
                    GlobFiles(x, "*.csproj"))
                .Where(x => !x.EndsWith("test.csproj"))
                .ForEach(x =>
                    DotNetPublish(s =>
                    {
                        s = s
                            .SetProject(x)
                            .SetConfiguration(Configuration)
                            .SetAssemblyVersion(GitVersion.AssemblySemVer)
                            .SetFileVersion(GitVersion.AssemblySemFileVer)
                            .SetInformationalVersion(GitVersion.InformationalVersion)
                            .SetAuthors("Il Harper")
                            .SetCopyright("2021 Il Harper")
                            .SetTitle("Ruminoid Toolbox")
                            .SetDescription("Visual Processing Toolbox.")
                            .SetRepositoryType("git")
                            .SetRepositoryUrl("https://github.com/Ruminoid/Toolbox.git");

                        // if (PublishRelease &&
                        //     (x.EndsWith("rmbox.csproj") ||
                        //      x.EndsWith("rmbox-shell.csproj")))
                        //     s = s
                        //         .SetRuntime(Runtime);
                        //         //.SetSelfContained(PublishRelease) // dotnet/sdk/issues/10902
                        //         //.EnablePublishReadyToRun() // PublishReadyToRunShowWarnings
                        //         //.EnablePublishTrimmed() // Plugin Load Error

                        return s;
                    }));

            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        });

    Target DevPluginsPublish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed

            new[]
                {
                    PluginsDirectory
                }
                .SelectMany(x =>
                    Directory.EnumerateDirectories(x))
                .SelectMany(x =>
                    GlobFiles(x, "*.csproj"))
                .Where(x => !x.EndsWith("test.csproj"))
                .ForEach(x =>
                    DotNetPublish(s =>
                    {
                        s = s
                            .SetProject(x)
                            .SetConfiguration(Configuration)
                            .SetAssemblyVersion(GitVersion.AssemblySemVer)
                            .SetFileVersion(GitVersion.AssemblySemFileVer)
                            .SetInformationalVersion(GitVersion.InformationalVersion)
                            .SetAuthors("Il Harper")
                            .SetCopyright("2021 Il Harper")
                            .SetTitle("Ruminoid Toolbox")
                            .SetDescription("Visual Processing Toolbox.")
                            .SetRepositoryType("git")
                            .SetRepositoryUrl("https://github.com/Ruminoid/Toolbox.git");

                        return s;
                    }));

            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        });

    Target Dev => _ => _
        .DependsOn(DevPluginsPublish)
        .Executes(() =>
        {
            Logger.Info("Cleaning output directory.");
            EnsureCleanDirectory(OutputDirectory);

            Logger.Info("Packing projects in src.");
            Directory.EnumerateDirectories(SourceDirectory).ToArray()
                .Where(x => !ExcludeSourceFolders.Any(x.EndsWith))
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput((AbsolutePath) x),
                        OutputDirectory));

            Logger.Info("Packing projects in plugins.");
            Directory.EnumerateDirectories(PluginsDirectory).ToArray()
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput(
                            (AbsolutePath)x,
                            ProjectOutputMode.Publish),
                        OutputDirectory / "plugins"));

            AbsolutePath localToolsTempDirectory = RootDirectory / "temp" / "tools";

            // ReSharper disable once InvertIf
            if (DirectoryExists(localToolsTempDirectory))
            {
                Logger.Info("Tools folder founded. Packing tools.");
                EnsureCleanDirectory(ToolsDirectory);
                ForceCopyDirectoryRecursively(localToolsTempDirectory, ToolsDirectory);
            }
        });

    Target PackBinary => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            Logger.Info("Cleaning output directory.");
            EnsureCleanDirectory(OutputDirectory);

            Logger.Info("Packing projects in src.");
            Directory.EnumerateDirectories(SourceDirectory).ToArray()
                .Where(x => !ExcludeSourceFolders.Any(x.EndsWith))
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput(
                            (AbsolutePath) x,
                            x.EndsWith("rmbox") || x.EndsWith("rmbox-shell")
                                ? ProjectOutputMode.PublishSelfContained
                                : ProjectOutputMode.Publish),
                        OutputDirectory));

            Logger.Info("Packing projects in plugins.");
            Directory.EnumerateDirectories(PluginsDirectory).ToArray()
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput(
                            (AbsolutePath)x,
                            ProjectOutputMode.Publish),
                        OutputDirectory / "plugins"));
        });

    Target PackTools => _ => _
        .DependsOn(PackBinary)
        .Executes(() =>
        {
            EnsureCleanDirectory(ToolsDirectory);

            DownloadTools();

            if (DirectoryExists(RootDirectory / "tools-gen"))
            {
                Logger.Info("tools-gen folder founded. Packing tools.");
                ForceCopyDirectoryRecursively(RootDirectory / "tools-gen", ToolsDirectory);
            }

            Logger.Success("PackTools completed.");

            //Logger.Info("Making soft link.");
            //ProcessTasks.StartShell(
            //        Platform == PlatformFamily.Windows ? "mklink 启动Toolbox rmbox\\rmbox.exe" : "ln -s ./rmbox/rmbox 启动Toolbox",
            //        PackDirectory)
            //    .AssertZeroExitCode();

            //Logger.Info("Making link.");
            //TextTasks.WriteAllText(
            //    PackDirectory / ("启动Toolbox" + (Platform == PlatformFamily.Windows ? ".bat" : ".sh")),
            //    Platform == PlatformFamily.Windows ? "@echo off\nrmbox\\rmbox.exe %*\n" : "./rmbox/rmbox $*\n");

            if (DirectoryExists(RootDirectory / "rmbenv-gen"))
            {
                Logger.Info("rmbenv-gen folder founded. Packing rmbenv.");
                ForceCopyDirectoryRecursively(RootDirectory / "rmbenv-gen", PackDirectory);
                Logger.Success("rmbenv packed.");
            }

            if (Platform == PlatformFamily.Linux ||
                Platform == PlatformFamily.OSX)
            {
                Logger.Info("Configuring privileges.");
                ProcessTasks.StartShell(
                        "chmod -R 775 tools",
                        TemporaryDirectory)
                    .AssertZeroExitCode();

                ProcessTasks.StartShell(
                        "chmod -R 775 RuminoidToolbox",
                        DistDirectory)
                    .AssertZeroExitCode();
            }

            Logger.Info("Compressing dist.");
            ProcessTasks.StartShell(
                    $"{ToolsTempDirectory / "7za"} a {DistDirectory / "rmbox.7z"} {PackDirectory}/ -mx9")
                .AssertZeroExitCode();
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Full => _ => _
        .DependsOn(PackTools, Test);

    AbsolutePath NavigateToProjectOutput(
        AbsolutePath absolutePath,
        ProjectOutputMode mode = ProjectOutputMode.Build) =>
        mode switch
        {
            ProjectOutputMode.Build =>
                absolutePath / "bin" / Configuration / "net5.0",
            ProjectOutputMode.Publish =>
                absolutePath / "bin" / Configuration / "net5.0" / "publish",
            ProjectOutputMode.PublishSelfContained =>
                absolutePath / "bin" / Configuration / "net5.0" / Runtime / "publish",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    void ForceCopyDirectoryRecursively(string source, string target) =>
        CopyDirectoryRecursively(
            source,
            target,
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.OverwriteIfNewer);

}

enum ProjectOutputMode
{
    Build = 0,
    Publish = 1,
    PublishSelfContained = 2
}
