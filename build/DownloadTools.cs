using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;

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
    const string PowerShellVersion = "7.0.5";
    const string NodejsVersion = "14.16.0";
    const string PythonVersion = "3.9.2";
    const string LuaVersion = "5.3.5";

    void DownloadToolsWindows()
    {
        Logger.Info("Downloading FFmpeg.");
        string ffmpegVersion =
            HttpTasks.HttpDownloadString(
                "https://www.gyan.dev/ffmpeg/builds/release-version");

        Logger.Info($"Downloading FFmpeg {ffmpegVersion}.");
        HttpTasks.HttpDownloadFile(
            $"https://github.com/GyanD/codexffmpeg/releases/download/{ffmpegVersion}/ffmpeg-{ffmpegVersion}-full_build.zip",
            ToolsTempDirectory / "ffmpeg.zip");

        AbsolutePath ffmpegExtractTempPath = ToolsTempDirectory / "ffmpeg";

        Logger.Info("Extracting FFmpeg.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "ffmpeg.zip",
            ffmpegExtractTempPath);

        ForceCopyDirectoryRecursively(
            ffmpegExtractTempPath / $"ffmpeg-{ffmpegVersion}-full_build" / "bin",
            ToolsDirectory);

        Logger.Info("Downloading x264.");
        HttpTasks.HttpDownloadFile(
            $"https://artifacts.videolan.org/x264/release-win64/x264-{X264Version}.exe",
            ToolsDirectory / "x264.exe");

        Logger.Info($"Downloading PowerShell v{PowerShellVersion}.");
        HttpTasks.HttpDownloadFile(
            $"https://github.com/PowerShell/PowerShell/releases/download/v{PowerShellVersion}/PowerShell-{PowerShellVersion}-win-x64.zip",
            ToolsTempDirectory / "pwsh.zip");

        Logger.Info("Extracting PowerShell.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "pwsh.zip",
            ToolsDirectory);

        Logger.Info("Downloading Node.js.");
        HttpTasks.HttpDownloadFile(
            $"https://nodejs.org/dist/v{NodejsVersion}/node-v{NodejsVersion}-win-x64.zip",
            ToolsTempDirectory / "node.zip");

        AbsolutePath nodeExtractTempPath = ToolsTempDirectory / "node";

        Logger.Info("Extracting Node.js.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "node.zip",
            nodeExtractTempPath);

        ForceCopyDirectoryRecursively(
            nodeExtractTempPath / $"node-v{NodejsVersion}-win-x64",
            ToolsDirectory);

        Logger.Info("Downloading Python.");
        HttpTasks.HttpDownloadFile(
            $"https://www.python.org/ftp/python/{PythonVersion}/python-{PythonVersion}-embed-amd64.zip",
            ToolsTempDirectory / "python.zip");

        Logger.Info("Extracting Python.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "python.zip",
            ToolsDirectory);

        Logger.Info("Downloading Lua.");
        HttpTasks.HttpDownloadFile(
            $"https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/lua-{LuaVersion}-win.zip",
            ToolsTempDirectory / "lua.zip");

        Logger.Info("Extracting Lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading json.lua.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/json.lua.zip",
            ToolsTempDirectory / "json.lua.zip");

        Logger.Info("Extracting json.lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "json.lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading 7za.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/develar/7zip-bin/master/win/x64/7za.exe",
            ToolsTempDirectory / "7za.exe");

        Logger.Info("Moving 7za.");
        CopyFileToDirectory(
            ToolsTempDirectory / "7za.exe",
            ToolsDirectory);
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

        Logger.Info($"Downloading PowerShell v{PowerShellVersion}.");
        HttpTasks.HttpDownloadFile(
            $"https://github.com/PowerShell/PowerShell/releases/download/v{PowerShellVersion}/PowerShell-{PowerShellVersion}-osx-x64.tar.gz",
            ToolsTempDirectory / "pwsh.tar.gz");

        Logger.Info("Extracting PowerShell.");
        CompressionTasks.UncompressTarGZip(
            ToolsTempDirectory / "pwsh.tar.gz",
            ToolsDirectory);

        Logger.Info("Downloading Node.js.");
        HttpTasks.HttpDownloadFile(
            $"https://nodejs.org/dist/v{NodejsVersion}/node-v{NodejsVersion}-darwin-x64.tar.gz",
            ToolsTempDirectory / "node.tar.gz");

        AbsolutePath nodeExtractTempPath = ToolsTempDirectory / "node";

        Logger.Info("Extracting Node.js.");
        CompressionTasks.UncompressTarGZip(
            ToolsTempDirectory / "node.tar.gz",
            nodeExtractTempPath);

        // You need to manually install npm.
        ForceCopyDirectoryRecursively(
            nodeExtractTempPath / $"node-v{NodejsVersion}-darwin-x64" / "bin",
            ToolsDirectory);

        // You need to compile Python yourself.

        Logger.Info("Downloading Lua.");
        HttpTasks.HttpDownloadFile(
            $"https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/lua-{LuaVersion}-osx.zip",
            ToolsTempDirectory / "lua.zip");

        Logger.Info("Extracting Lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading json.lua.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/json.lua.zip",
            ToolsTempDirectory / "json.lua.zip");

        Logger.Info("Extracting json.lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "json.lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading 7za.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/develar/7zip-bin/master/mac/x64/7za",
            ToolsTempDirectory / "7za");

        Logger.Info("Moving 7za.");
        CopyFileToDirectory(
            ToolsTempDirectory / "7za",
            ToolsDirectory);
    }

    void DownloadToolsLinux()
    {
        Logger.Info("Downloading FFmpeg.");
        HttpTasks.HttpDownloadFile(
            "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz",
            ToolsTempDirectory / "ffmpeg.tar.xz");

        // You need to extract "ffmpeg.tar.xz" yourself.
        CopyFileToDirectory(
            ToolsTempDirectory / "ffmpeg.tar.xz",
            ToolsDirectory);

        Logger.Info("Downloading x264.");
        HttpTasks.HttpDownloadFile(
            $"https://artifacts.videolan.org/x264/release-debian-amd64/x264-{X264Version}",
            ToolsDirectory / "x264.exe");

        Logger.Info($"Downloading PowerShell v{PowerShellVersion}.");
        HttpTasks.HttpDownloadFile(
            $"https://github.com/PowerShell/PowerShell/releases/download/v{PowerShellVersion}/PowerShell-{PowerShellVersion}-linux-x64.tar.gz",
            ToolsTempDirectory / "pwsh.tar.gz");

        Logger.Info("Extracting PowerShell.");
        CompressionTasks.UncompressTarGZip(
            ToolsTempDirectory / "pwsh.tar.gz",
            ToolsDirectory);

        Logger.Info("Downloading Node.js.");
        HttpTasks.HttpDownloadFile(
            $"https://nodejs.org/dist/v{NodejsVersion}/node-v{NodejsVersion}-linux-x64.tar.xz",
            ToolsTempDirectory / "node.tar.xz");

        // You need to extract "node.tar.xz" yourself.
        CopyFileToDirectory(
            ToolsTempDirectory / "node.tar.xz",
            ToolsDirectory);

        // You need to compile Python yourself.

        Logger.Info("Downloading Lua.");
        HttpTasks.HttpDownloadFile(
            $"https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/lua-{LuaVersion}-linux.zip",
            ToolsTempDirectory / "lua.zip");

        Logger.Info("Extracting Lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading json.lua.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/Afanyiyu/Delivr/master/lua/json.lua.zip",
            ToolsTempDirectory / "json.lua.zip");

        Logger.Info("Extracting json.lua.");
        CompressionTasks.UncompressZip(
            ToolsTempDirectory / "json.lua.zip",
            ToolsDirectory);

        Logger.Info("Downloading 7za.");
        HttpTasks.HttpDownloadFile(
            "https://raw.githubusercontent.com/develar/7zip-bin/master/linux/x64/7za",
            ToolsTempDirectory / "7za");

        Logger.Info("Moving 7za.");
        CopyFileToDirectory(
            ToolsTempDirectory / "7za",
            ToolsDirectory);
    }
}
