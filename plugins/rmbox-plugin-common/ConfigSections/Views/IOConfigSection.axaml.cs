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
        IOConfigSectionId,
        "输入/输出")]
    public sealed class IOConfigSection : ConfigSectionBase
    {
        public IOConfigSection()
        {
            throw new InvalidOperationException();
        }

        public IOConfigSection(
            JToken sectionConfig)
        {
            DataContext = new IOConfigSectionViewModel(this, sectionConfig);

            InitializeComponent();

            _inputFileGrid = this.FindControl<Grid>("InputFileGrid");

            AddHandler(DragDrop.DropEvent, DropHandler);
        }

        private void DropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as IOConfigSectionViewModel).Input = e.Data.GetText();
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as IOConfigSectionViewModel).Input = (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault();
        }

        private readonly Grid _inputFileGrid;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #region Data

        public override IOConfigSectionViewModel Config => DataContext as IOConfigSectionViewModel;

        #endregion
    }
}
