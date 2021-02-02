using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
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
    public static int Main () => Execute<Build>(x => x.PackCore);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    bool PublishRelease => Equals(Configuration, Configuration.Release);

    [Parameter(".NET Runtime ID")]
    readonly string Runtime = "win-x64";

    [Solution("rmbox.sln")] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework="netcoreapp3.1")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath PluginsDirectory => RootDirectory / "plugins";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath DistDirectory => RootDirectory / "dist";
    AbsolutePath OutputDirectory => DistDirectory / "rmbox";
    AbsolutePath ToolsDirectory => OutputDirectory / "tools";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
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
                .SetRuntime(Runtime)
                .SetPublishReadyToRun(PublishRelease)
                .SetPublishTrimmed(PublishRelease)
                .EnableNoRestore());
        });

    Target PackCore => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Logger.Info("Cleaning output directory.");
            EnsureCleanDirectory(OutputDirectory);

            Logger.Info("Packing projects in src.");
            Directory.EnumerateDirectories(SourceDirectory).ToArray()
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput((AbsolutePath) x),
                        OutputDirectory));

            Logger.Info("Packing projects in plugins.");
            Directory.EnumerateDirectories(PluginsDirectory).ToArray()
                .ForEach(x =>
                    ForceCopyDirectoryRecursively(
                        NavigateToProjectOutput((AbsolutePath) x),
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

    Target PackTools => _ => _
        .DependsOn(PackCore)
        .Executes(() =>
        {
            EnsureCleanDirectory(ToolsDirectory);

            DownloadTools();
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

    AbsolutePath NavigateToProjectOutput(AbsolutePath absolutePath) =>
        absolutePath / "bin" / Configuration / "net5.0";

    void ForceCopyDirectoryRecursively(string source, string target) =>
        CopyDirectoryRecursively(
            source,
            target,
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.OverwriteIfNewer);

}
