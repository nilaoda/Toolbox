using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PictureFlowConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public PictureFlowConfigSectionViewModel(
            PictureFlowConfigSection view,
            JToken sectionConfig)
        {
            _view = view;

            SupportPicture = sectionConfig["support_picture"]?.ToObject<bool>() ?? false;

            _hasInvalidCharHelper = this
                .WhenAnyValue(x => x.Picture)
                .Select(x => PathExtension.InvalidChars.Any(x.Contains))
                .ToProperty(this, x => x.HasInvalidChar);
        }

        #endregion

        #region View

        private readonly PictureFlowConfigSection _view;

        #endregion

        #region Config

        public bool SupportPicture { get; }

        #endregion

        #region Data

        [JsonProperty("picture")]
        private string _picture = "";

        public string Picture
        {
            get => _picture;
            set => this.RaiseAndSetIfChanged(ref _picture, value);
        }

        [JsonProperty("frame_rate")]
        private int _frameRate = 30;

        public int FrameRate
        {
            get => _frameRate;
            set => this.RaiseAndSetIfChanged(ref _frameRate, value);
        }

        [JsonProperty("crf_value")]
        private double _crfValue = 25;

        public double CrfValue
        {
            get => _crfValue;
            set => this.RaiseAndSetIfChanged(ref _crfValue, value);
        }

        [JsonProperty("size")]
        private string _size = "1920x1080";

        public string Size
        {
            get => _size;
            set => this.RaiseAndSetIfChanged(ref _size, value);
        }

        [JsonProperty("duration")]
        private int _duration = 60;

        public int Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }

        #endregion

        #region Display

        private readonly ObservableAsPropertyHelper<bool> _hasInvalidCharHelper;

        public bool HasInvalidChar => _hasInvalidCharHelper.Value;

        #endregion

        #region Operations

        public async Task PickFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "选择图片文件"
            };
            string[] result = await dialog.ShowAsync((Window)_view.GetVisualRoot());

            if (!result.Any() ||
                string.IsNullOrWhiteSpace(result[0]))
                return;

            Picture = result[0];
        }

        #endregion
    }
}
