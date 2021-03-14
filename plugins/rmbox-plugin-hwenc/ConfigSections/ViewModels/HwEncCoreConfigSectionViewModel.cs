using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HwEncCoreConfigSectionViewModel : ReactiveObject
    {
        #region List

        public List<string> CoreList { get; } =
            Directory.EnumerateFiles(
                    StorageHelper.GetSectionFolderPath("tools"),
                    "*")
                .Select(Path.GetFileName)
                .Where(x =>
                    x.StartsWith("nvencc") ||
                    x.StartsWith("qsvencc") ||
                    x.StartsWith("vceencc"))
                .Where(x => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || x.EndsWith(".exe"))
                .Select(x => x.EndsWith(".exe") ? x[..^4] : x)
                .Select(x => x.ToLower())
                .ToList();

        #endregion

        [JsonProperty("core")]
        private string _core = "nvencc64";

        public string Core
        {
            get => _core;
            set => this.RaiseAndSetIfChanged(ref _core, value);
        }
    }
}
