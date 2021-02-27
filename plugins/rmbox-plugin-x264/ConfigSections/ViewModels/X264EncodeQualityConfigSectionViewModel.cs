using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class X264EncodeQualityConfigSectionViewModel : ReactiveObject
    {
        #region Data

        [JsonProperty("encode_mode")]
        private string _encodeMode = "crf";

        public string EncodeMode
        {
            get => _encodeMode;
            set => this.RaiseAndSetIfChanged(ref _encodeMode, value);
        }

        [JsonProperty("crf_value")]
        private double _crfValue = 21;

        public double CrfValue
        {
            get => _crfValue;
            set => this.RaiseAndSetIfChanged(ref _crfValue, value);
        }

        [JsonProperty("2pass_value")]
        private double _twoPassValue = 5000;

        public double TwoPassValue
        {
            get => _twoPassValue;
            set => this.RaiseAndSetIfChanged(ref _twoPassValue, value);
        }

        #endregion

        #region Commands

        public void ApplyCrfPreset(double value) => CrfValue = value;

        public void ApplyTwoPassPreset(double value) => TwoPassValue = value;

        #endregion
    }
}
