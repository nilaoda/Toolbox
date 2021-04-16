using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Utils;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class X264CoreConfigSectionViewModel : ReactiveObject
    {
        #region List

        public List<string> CoreList { get; } =
            Directory.EnumerateFiles(
                    StorageHelper.GetSectionFolderPath("tools"),
                    "*")
                .Select(Path.GetFileName)
                .Where(x => x.StartsWith("x264"))
                .Where(x => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || x.EndsWith(".exe"))
                .Select(x => x.EndsWith(".exe") ? x[..^4] : x)
                .ToList();

        #endregion

        [JsonProperty("core")]
        private string _core = "x264";

        public string Core
        {
            get => _core;
            set => this.RaiseAndSetIfChanged(ref _core, value);
        }
    }
}
