using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core.Parser
{
    public interface IParser
    {
        /// <summary>
        /// 解析 JSON 项目文件。
        /// </summary>
        /// <param name="path">JSON 项目文件。</param>
        public List<(string Target, string Args, string Formatter)> Parse(JToken project);
    }
}
