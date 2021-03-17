using Ruminoid.Toolbox.Formatting;

namespace Ruminoid.Toolbox.Plugins.Common.Formatters
{
    [Formatter("null")]
    public class NullFormatter : IFormatter
    {
        public FormattedEvent Format(string target, string data)
        {
            return new(target, 0, true, "正在运行命令", data);
        }
    }
}
