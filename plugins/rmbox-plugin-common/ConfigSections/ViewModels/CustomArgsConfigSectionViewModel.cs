using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CustomArgsConfigSectionViewModel : ReactiveObject
    {
        #region Bindings

        private bool _useCustom;

        public bool UseCustom
        {
            get => _useCustom;
            set
            {
                if (_useCustom == value)
                    return;

                _args = value ? DisplayArgs : "";

                _useCustom = value;
                this.RaisePropertyChanged(nameof(UseCustom));
            }
        }

        private string _displayArgs = "";

        public string DisplayArgs
        {
            get => _displayArgs;
            set
            {
                if (_displayArgs == value)
                    return;

                _displayArgs = value;
                if (UseCustom) _args = value;

                this.RaisePropertyChanged(nameof(DisplayArgs));
            }
        }

        #endregion

        #region Data

        [JsonProperty("args")]
        private string _args = "";

        #endregion
    }
}
