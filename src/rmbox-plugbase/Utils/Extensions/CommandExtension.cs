using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static class CommandExtension
    {
        public static TaskCommand GenerateTryDeleteCommand(
            string path) =>
            new("pwsh",
                "-Command If (Test-Path " + path + " ) { Remove-Item " + path + " }",
                "null");
    }
}
