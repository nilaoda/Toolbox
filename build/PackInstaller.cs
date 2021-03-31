using System;
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

partial class Build : NukeBuild
{
    Target PackMac => _ => _
        .Executes(() =>
        {
            AbsolutePath MacDirectory = DistDirectory / "RuminoidToolbox.app";
            AbsolutePath MacContentsDirectory = MacDirectory / "Contents";
            AbsolutePath MacMacOSDirectory = MacDirectory / "MacOS";
            AbsolutePath MacResourcesDirectory = MacDirectory / "Resources";

            new[]
            {
                MacDirectory,
                MacContentsDirectory,
                MacMacOSDirectory,
                MacResourcesDirectory
            }.ForEach(x => EnsureCleanDirectory(x));

            Logger.Info("Copying contents.");
            ForceCopyDirectoryRecursively(
                PackDirectory,
                MacMacOSDirectory);

            Logger.Info("Copying icon.");
            CopyFileToDirectory(
                RootDirectory / "common" / "Assets" / "rmbox.icns",
                MacResourcesDirectory,
                FileExistsPolicy.Overwrite);

            new Plist
            {
                CFBundleName = "Toolbox",
                CFBundleDisplayName = "Ruminoid Toolbox",
                CFBundleSpokenName = "Ruminoid Toolbox",
                CFBundleIdentifier = "world.ruminoid.toolbox",
                LSApplicationCategoryType = "public.app-category.developer-tools",
                CFBundleVersion = GitVersion.MajorMinorPatch,
                CFBundleExecutable = "rmbenv",
                CFBundleIconFileName = "rmbox.icns",
                CFBundleShortVersionString = GitVersion.MajorMinorPatch
            }.Write(MacContentsDirectory);

            Logger.Info("Configuring privileges.");
            ProcessTasks.StartShell(
                    "chmod -R 775 RuminoidToolbox.app",
                    DistDirectory)
                .AssertZeroExitCode();
        });
}
