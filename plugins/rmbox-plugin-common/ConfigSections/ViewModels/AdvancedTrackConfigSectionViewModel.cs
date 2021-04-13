using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AdvancedTrackConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("process_mode")]
        private string _processMode = "auto";

        public string ProcessMode
        {
            get => _processMode;
            set => this.RaiseAndSetIfChanged(ref _processMode, value);
        }

        #endregion
    }
}
