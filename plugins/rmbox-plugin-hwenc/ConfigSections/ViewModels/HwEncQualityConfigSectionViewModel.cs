using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HwEncQualityConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("encode_mode")]
        private string _encodeMode = "cqp";

        public string EncodeMode
        {
            get => _encodeMode;
            set => this.RaiseAndSetIfChanged(ref _encodeMode, value);
        }

        [JsonProperty("cqp_value")]
        private string _cqpValue = "16:18:20";

        public string CqpValue
        {
            get => _cqpValue;
            set => this.RaiseAndSetIfChanged(ref _cqpValue, value);
        }

        [JsonProperty("2pass_value")]
        private int _twoPassValue = 5000;

        public int TwoPassValue
        {
            get => _twoPassValue;
            set => this.RaiseAndSetIfChanged(ref _twoPassValue, value);
        }

        #endregion

        #region Commands

        //public void ApplyCbrPreset(int value) => CbrValue = value;

        #endregion
    }
}
