using System;
using System.Composition;

namespace Ruminoid.Toolbox.Core
{
    [Export]
    public class ProjectParser
    {
        /// <summary>
        /// 解析 JSON 项目文件，并执行操作。
        /// </summary>
        /// <param name="path">JSON 项目文件的路径。</param>
        public void Parse(string path)
        {
            throw new NotImplementedException();
        }
    }
}
