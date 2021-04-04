using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    class OutputConfigSectionViewModel : ReactiveObject
    {
        #region Constructor

        public OutputConfigSectionViewModel(
            OutputConfigSection view)
        {
            _view = view;
        }

        #endregion

        #region Data

        [JsonProperty("output")]
        private string _output = "";

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }

        #endregion

        #region View

        private readonly OutputConfigSection _view;

        #endregion

        #region Operations

        public async Task SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "选择输出文件",
                DefaultExtension = ".mp4"
            };

            var result = await dialog.ShowAsync((Window)_view.GetVisualRoot());

            if (string.IsNullOrWhiteSpace(result)) return;

            Output = result;
        }

        #endregion
    }
}
