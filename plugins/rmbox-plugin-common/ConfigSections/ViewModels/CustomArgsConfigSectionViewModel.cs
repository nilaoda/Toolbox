using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CustomArgsConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public CustomArgsConfigSectionViewModel(
            JToken sectionConfig)
        {
            _defaultArgs = sectionConfig["default_args"]?.ToObject<string>() ?? string.Empty;
            _args = _defaultArgs;
        }

        #endregion

        private readonly string _defaultArgs;

        #region Bindings

        private bool _useCustom;

        public bool UseCustom
        {
            get => _useCustom;
            set
            {
                if (_useCustom == value)
                    return;

                Args = value ? _args : _defaultArgs;

                _useCustom = value;
                this.RaisePropertyChanged(nameof(UseCustom));
            }
        }

        [JsonProperty("args")]
        private string _args;

        public string Args
        {
            get => _args;
            set => this.RaiseAndSetIfChanged(ref _args, value);
        }

        #endregion
    }
}
