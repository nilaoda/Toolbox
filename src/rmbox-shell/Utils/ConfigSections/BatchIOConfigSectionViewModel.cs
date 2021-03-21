using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using DynamicData;
using Newtonsoft.Json;
using ReactiveUI;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Shell.Utils.ConfigSections
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BatchIOConfigSectionViewModel : ReactiveObject
    {
        #region Constructors

        public BatchIOConfigSectionViewModel(
            BatchIOConfigSection view)
        {
            _view = view;

            _isInputListSelected = this
                .WhenAnyValue(x => x.SelectedInput)
                .Select(x => x is not null)
                .ToProperty(this, x => x.IsInputListSelected);
        }

        /// <summary>
        /// 复制一个 BatchIOConfigSectionViewModel 的副本。
        /// </summary>
        /// <param name="origin">原实例。</param>
        public BatchIOConfigSectionViewModel(
            BatchIOConfigSectionViewModel origin)
        {
            InputList = new(origin.InputList);
            _subtitleFormat = origin.SubtitleFormat;
            _outputFormat = origin.OutputFormat;
            _useVsfm = origin.UseVsfm;
        }

        #endregion

        private readonly BatchIOConfigSection _view;

        #region Data

        [JsonProperty("inputs")]
        public ObservableCollection<string> InputList { get; } = new();

        [JsonProperty("subtitle_format")]
        private string _subtitleFormat = "{folder}{name}.ass";

        public string SubtitleFormat
        {
            get => _subtitleFormat;
            set => this.RaiseAndSetIfChanged(ref _subtitleFormat, value);
        }

        [JsonProperty("output_format")]
        private string _outputFormat = "{folder}{name}_output{extension}";

        public string OutputFormat
        {
            get => _outputFormat;
            set => this.RaiseAndSetIfChanged(ref _outputFormat, value);
        }

        [JsonProperty("use_vsfm")]
        private bool _useVsfm;

        public bool UseVsfm
        {
            get => _useVsfm;
            set => this.RaiseAndSetIfChanged(ref _useVsfm, value);
        }

        #endregion

        #region Bindings

        private string _selectedInput;

        public string SelectedInput
        {
            get => _selectedInput;
            set => this.RaiseAndSetIfChanged(ref _selectedInput, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isInputListSelected;

        public bool IsInputListSelected => _isInputListSelected.Value;

        #endregion

        #region Commands

        public async void DoAddFile()
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = true,
                Title = "添加文件"
            };

            string[] result = await dialog.ShowAsync((Window) _view.GetVisualRoot());

            InputList.AddRange(result);
        }

        public async void DoAddFolder()
        {
            OpenFolderDialog dialog = new()
            {
                Title = "添加文件夹"
            };

            string result = await dialog.ShowAsync((Window) _view.GetVisualRoot());

            InputList.AddRange(PathExtension.GetAllFiles(result));
        }

        public void DoRemoveFile()
        {
            if (!string.IsNullOrEmpty(SelectedInput) &&
                InputList.Contains(SelectedInput))
                InputList.Remove(SelectedInput);
        }

        public void DoClear()
        {
            InputList.Clear();
        }

        #endregion
    }
}
