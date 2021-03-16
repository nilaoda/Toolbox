using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IOConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public IOConfigSectionViewModel(
            IOConfigSection view)
        {
            _view = view;
        }

        #endregion

        #region View

        private readonly IOConfigSection _view;

        #endregion

        #region Data

        [JsonProperty("input")]
        private string _input = "";

        public string Input
        {
            get => _input;
            set => this.RaiseAndSetIfChanged(ref _input, value);
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
