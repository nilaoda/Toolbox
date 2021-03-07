using CommandLine;

namespace Ruminoid.Toolbox.Helpers.CommandLine
{
    [Verb("process", true, HelpText = "执行任务或任务队列。")]
    public class ProcessOptions
    {
        [Value(1, HelpText = "JSON 项目文件的路径。", Required = true)]
        public string ProjectPath { get; set; }

        [Option("skip-version-check", Default = false, HelpText = "跳过项目文件版本检查。", Required = false)]
        public bool SkipVersionCheck { get; set; }

        [Option('o', "log-process-out", Default = false, HelpText = "显示进程输出。", Required = false)]
        public bool LogProcessOut { get; set; }

        [Option('h', "hide-formatted-out", Default = false, HelpText = "隐藏标准输出。", Required = false)]
        public bool HideFormattedOutput { get; set; }

        [Option('d', "dynamic-link", Default = 0, HelpText = "动态连接到Rmbox Shell。", Required = false)]
        public int DynamicLinkPort { get; set; }
    }
}
