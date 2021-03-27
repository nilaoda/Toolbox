namespace Ruminoid.Toolbox.Utils.Extensions
{
    public static class CommandExtension
    {
        public static (string Target, string Args, string Formatter) GenerateTryDeleteCommand(
            string path) =>
            new("pwsh",
                "-Command If (Test-Path " + path + " ) { Remove-Item " + path + " }",
                "null");
    }
}
