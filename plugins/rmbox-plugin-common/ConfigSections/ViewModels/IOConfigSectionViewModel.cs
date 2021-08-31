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
    public class IOConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public IOConfigSectionViewModel(
            IOConfigSection view,
            JToken sectionConfig)
        {
            _view = view;

            SupportSubtitle = sectionConfig["support_subtitle"]?.ToObject<bool>() ?? false;

            _outputSuffix = sectionConfig["output_suffix"]?.ToObject<string>() ?? _outputSuffix;
            _outputExtension = sectionConfig["output_extension"]?.ToObject<string>() ?? _outputExtension;

            _hasInvalidCharHelper =
                // ReSharper disable once InvokeAsExtensionMethod
                Observable.Merge(
                        this
                            .WhenAnyValue(x => x.Input)
                            .Select(x => PathExtension.InvalidChars.Any(x.Contains) || x.Length > 90),
                        this
                            .WhenAnyValue(x => x.Subtitle)
                            .Select(x => PathExtension.InvalidChars.Any(x.Contains) || x.Length > 90))
                    .ToProperty(this, x => x.HasInvalidChar);
        }

        #endregion

        #region View

        private readonly IOConfigSection _view;

        #endregion

        #region Config

        public bool SupportSubtitle { get; }

        #endregion

        #region Data

        [JsonProperty("input")]
        private string _input = "";

        public string Input
        {
            get => _input;
            set
            {
                this.RaiseAndSetIfChanged(ref _input, value);

                if (UseCustomOutput) return;
                this.RaiseAndSetIfChanged(ref _output, value.Suffix(_outputSuffix, _outputExtension), nameof(Output));
            }
        }

        [JsonProperty("subtitle")]
        private string _subtitle = "";

        public string Subtitle
        {
            get => _subtitle;
            set => this.RaiseAndSetIfChanged(ref _subtitle, value);
        }

        [JsonProperty("output")]
        private string _output = "";

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }

        #endregion

        #region AutoFill

        private string _outputSuffix = "_output";
        private string _outputExtension;

        private bool _useCustomOutput;

        public bool UseCustomOutput
        {
            get => _useCustomOutput;
            set
            {
                this.RaiseAndSetIfChanged(ref _useCustomOutput, value);

                if (!value)
                    this.RaiseAndSetIfChanged(ref _output, Input.Suffix(_outputSuffix, _outputExtension),
                        nameof(Output));
            }
        }

        #endregion

        #region Display

        private readonly ObservableAsPropertyHelper<bool> _hasInvalidCharHelper;

        public bool HasInvalidChar => _hasInvalidCharHelper.Value;

        #endregion

        #region Operations

        public async Task PickFile(string field)
        {
            string title = field switch
            {
                "Input" => "输入",
                "Subtitle" => "字幕",
                _ => ""
            };

            OpenFileDialog dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = $"选择{title}文件"
            };
            string[] result = await dialog.ShowAsync((Window) _view.GetVisualRoot());

            if (!result.Any() ||
                string.IsNullOrWhiteSpace(result[0]))
                return;

            switch (field)
            {
                case "Input":
                    Input = result[0];
                    break;
                case "Subtitle":
                    Subtitle = result[0];
                    break;
            }
        }

        public async Task SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "选择输出文件",
                DefaultExtension = ".mp4"
            };

            var result = await dialog.ShowAsync((Window) _view.GetVisualRoot());

            if (string.IsNullOrWhiteSpace(result)) return;

            Output = result;
        }

        #endregion
    }
}
