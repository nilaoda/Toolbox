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

partial class Build
{
    readonly AbsolutePath ToolsTempDirectory = TemporaryDirectory / "tools";

    void DownloadTools()
    {
        
        EnsureCleanDirectory(ToolsTempDirectory);

        Logger.Info($"Downloading tools for {Platform}");
        switch (Platform)
        {
            case PlatformFamily.Windows:
                DownloadToolsWindows();
                break;
            case PlatformFamily.Linux:
                DownloadToolsLinux();
                break;
            case PlatformFamily.OSX:
                DownloadToolsMacos();
                break;
            default:
                Logger.Warn("Unknown platform detected. Skipping download.");
                break;
        }

        Logger.Success("All tools downloaded.");
    }

    const string X264Version = "r3043-59c0609";

    void DownloadToolsWindows()
    {
        Logger.Info("Downloading FFmpeg.");
        string ffmpegVersion =
            HttpTasks.HttpDownloadString(
                "https://www.gyan.dev/ffmpeg/builds/release-version");

        Logger.Info($"Downloading FFmpeg {ffmpegVersion}");
        HttpTasks.HttpDownloadFile(
            $"https://github.com/GyanD/codexffmpeg/releases/download/{ffmpegVersion}/ffmpeg-{ffmpegVersion}-full_build.zip",
            ToolsTempDirectory / "ffmpeg.zip");

        Logger.Info("Extracting FFmpeg.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "ffmpeg.zip",
            ToolsDirectory);

        Logger.Info("Downloading x264.");
        HttpTasks.HttpDownloadFile(
            $"https://artifacts.videolan.org/x264/release-win64/x264-{X264Version}.exe",
            ToolsDirectory / "x264.exe");
    }

    void DownloadToolsMacos()
    {
        Logger.Info("Downloading FFmpeg.");

        new[]
        {
            "ffmpeg",
            "ffprobe",
            "ffplay",
            "ffserver"
        }.ForEach(x =>
        {
            Logger.Info($"Downloading {x}.");
            HttpTasks.HttpDownloadFile(
                $"https://evermeet.cx/ffmpeg/getrelease/{x}/zip",
                ToolsTempDirectory / $"{x}.zip");

            Logger.Info($"Extracting {x}.");
            CompressionTasks.UncompressZip(
                ToolsTempDirectory / $"{x}.zip",
                ToolsDirectory);
        });
        
        Logger.Info("Downloading x264.");
        HttpTasks.HttpDownloadFile(
            $"https://artifacts.videolan.org/x264/release-macos/x264-{X264Version}",
            ToolsDirectory / "x264.exe");
    }

    void DownloadToolsLinux()
    {
        Logger.Info("Downloading FFmpeg.");
        HttpTasks.HttpDownloadFile(
            "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz",
            ToolsTempDirectory / "ffmpeg.tar.xz");

        //Logger.Info("Extracting FFmpeg.");
        //CompressionTasks.Uncompress(
        //    ToolsTempDirectory / "ffmpeg.tar.xz",
        //    ToolsDirectory);

        CopyFileToDirectory(
            ToolsTempDirectory / "ffmpeg.tar.xz",
            ToolsDirectory);
        Logger.Warn("You need to extract \"ffmpeg.tar.xz\" yourself.");

        Logger.Info("Downloading x264.");
        HttpTasks.HttpDownloadFile(
            $"https://artifacts.videolan.org/x264/release-debian-amd64/x264-{X264Version}",
            ToolsDirectory / "x264.exe");
    }
}
