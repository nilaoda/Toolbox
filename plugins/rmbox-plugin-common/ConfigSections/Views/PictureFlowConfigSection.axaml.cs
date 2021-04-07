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
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.PictureFlowConfigSection",
        "一图流")]
    public class PictureFlowConfigSection : ConfigSectionBase
    {
        public PictureFlowConfigSection()
        {
            throw new InvalidOperationException();
        }

        public PictureFlowConfigSection(
            JToken sectionConfig)
        {
            DataContext = new PictureFlowConfigSectionViewModel(this, sectionConfig);

            InitializeComponent();

            _pictureFileGrid = this.FindControl<Grid>("PictureFileGrid");

            _pictureFileGrid.AddHandler(DragDrop.DropEvent, PictureDropHandler);
        }

        private void PictureDropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as PictureFlowConfigSectionViewModel).Picture = e.Data.GetText();
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as PictureFlowConfigSectionViewModel).Picture = (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly Grid _pictureFileGrid;

        public override object Config => DataContext as PictureFlowConfigSectionViewModel;
    }
}
