using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core.Parser
{
    public interface IParser
    {
        /// <summary>
        /// 解析 JSON 项目文件。
        /// </summary>
        /// <param name="project">JSON 项目文件。</param>
        public List<TaskCommand> Parse(JToken project);
    }
}
