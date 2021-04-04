using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

partial class Build
{
    Target PackMac => _ => _
        .Executes(() =>
        {
            AbsolutePath MacDirectory = DistDirectory / "RuminoidToolbox.app";
            AbsolutePath MacContentsDirectory = MacDirectory / "Contents";
            AbsolutePath MacMacOSDirectory = MacContentsDirectory / "MacOS";
            AbsolutePath MacResourcesDirectory = MacContentsDirectory / "Resources";

            new[]
            {
                MacDirectory,
                MacContentsDirectory,
                MacMacOSDirectory,
                MacResourcesDirectory
            }.ForEach(EnsureCleanDirectory);

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
                CFBundleIconFile = "rmbox.icns",
                CFBundleShortVersionString = GitVersion.MajorMinorPatch
            }.Write(MacContentsDirectory);

            Logger.Info("Configuring privileges.");
            ProcessTasks.StartShell(
                    "chmod -R 775 RuminoidToolbox.app",
                    DistDirectory)
                .AssertZeroExitCode();
        });

    Target PackWindows => _ => _
        .Executes(() =>
        {
            ProcessTasks.StartShell(
                    $"makensis -DVersion={GitVersion.MajorMinorPatch} common/installer/nsis/installer.nsi")
                .AssertZeroExitCode();
        });
}
