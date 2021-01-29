using System;
using System.Composition;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProjectParser
    {
        /// <summary>
        /// 解析 JSON 项目文件，并执行操作。
        /// </summary>
        /// <param name="project">JSON 项目文件。</param>
        public void Parse(JObject project)
        {
            throw new NotImplementedException();
        }
    }
}
