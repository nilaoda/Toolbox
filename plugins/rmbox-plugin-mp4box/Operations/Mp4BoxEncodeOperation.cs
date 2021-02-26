using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.Mp4Box.Operations
{
    [Operation(
        "Ruminoid.Toolbox.Plugins.Mp4Box.Operations.Mp4BoxEncodeOperation",
        "小丸压制",
        "使用小丸压制法进行视频压制。")]
    public class Mp4BoxEncodeOperation : IOperation
    {
        public List<Tuple<string, string>> Generate(Dictionary<string, JToken> sectionData)
        {
            JToken ioSection = sectionData["Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"];

            return new List<Tuple<string, string>>
            {
            };
        }

        public List<string> RequiredConfigSections => new()
        {
            "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection"
        };
    }
}
