using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection",
        "混流")]
    public class MuxConfigSection : ConfigSectionBase
    {
        public MuxConfigSection()
        {
            throw new InvalidOperationException();
        }

        public MuxConfigSection(
            JToken sectionConfig)
        {
            DataContext = new MuxConfigSectionViewModel(this, sectionConfig);

            InitializeComponent();

            _videoFileGrid = this.FindControl<Grid>("VideoFileGrid");
            _audioFileGrid = this.FindControl<Grid>("AudioFileGrid");
            _subtitleGrid = this.FindControl<Grid>("SubtitleFileGrid");

            _videoFileGrid.AddHandler(DragDrop.DropEvent, VideoDropHandler);
            _audioFileGrid.AddHandler(DragDrop.DropEvent, AudioDropHandler);
            _subtitleGrid.AddHandler(DragDrop.DropEvent, SubtitleDropHandler);
        }

        private void VideoDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as MuxConfigSectionViewModel).Video = e.Data.GetText();
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as MuxConfigSectionViewModel).Video = (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault();
        }

        private void AudioDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as MuxConfigSectionViewModel).Audio = e.Data.GetText();
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as MuxConfigSectionViewModel).Audio = (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault();
        }

        private void SubtitleDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as MuxConfigSectionViewModel).Subtitle = e.Data.GetText();
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as MuxConfigSectionViewModel).Subtitle = (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault();
        }

        private readonly Grid _videoFileGrid;
        private readonly Grid _audioFileGrid;
        private readonly Grid _subtitleGrid;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as MuxConfigSectionViewModel;
    }
}
