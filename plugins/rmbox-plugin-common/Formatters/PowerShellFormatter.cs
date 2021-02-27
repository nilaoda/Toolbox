using Ruminoid.Toolbox.Formatting;

namespace Ruminoid.Toolbox.Plugins.Common.Formatters
{
    [Formatter("pwsh")]
    public class PowerShellFormatter : IFormatter
    {
        public FormattedEvent Format(string target, string data)
        {
            return new(target, 0, true, "正在运行脚本", data);
        }
    }
}
