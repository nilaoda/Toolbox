using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MuxOptionsConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("use_shortest")]
        private bool _useShortest;

        public bool UseShortest
        {
            get => _useShortest;
            set => this.RaiseAndSetIfChanged(ref _useShortest, value);
        }

        #endregion
    }
}
