using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Audio.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AudioQualityConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("bitrate")]
        private int _bitrate = 320;

        public int Bitrate
        {
            get => _bitrate;
            set => this.RaiseAndSetIfChanged(ref _bitrate, value);
        }

        #endregion
    }
}
