using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HwEncCodecConfigSectionViewModel : ReactiveObject
    {
        #region List

        private readonly Dictionary<string, string> _codecDictionary = new()
        {
            {"h264", "AVC（H.264）"},
            {"h265", "HEVC（H.265）"}
        };

        private List<string> _codecList;

        public List<string> CodecList => _codecList ??= _codecDictionary.Values.ToList();

        #endregion

        [JsonProperty("codec")]
        private string _codec = "h264";

        public string Codec
        {
            get => _codecDictionary[_codec];
            set
            {
                string key = _codecDictionary.FirstOrDefault(x => x.Value == value).Key;

                if (key == _codec) return;

                _codec = key;
                this.RaisePropertyChanged(nameof(Codec));
            }
        }
    }
}
