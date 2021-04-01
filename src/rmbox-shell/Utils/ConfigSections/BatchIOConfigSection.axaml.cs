using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DynamicData;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Shell.Utils.ConfigSections
{
    [ConfigSection(
        BatchIOConfigSectionId,
        "批量")]
    public class BatchIOConfigSection : ConfigSectionBase
    {
        public BatchIOConfigSection()
        {
            DataContext = new BatchIOConfigSectionViewModel(this);

            InitializeComponent();

            _inputFileGrid = this.FindControl<Grid>("InputFileGrid");

            AddHandler(DragDrop.DropEvent, DropHandler);
        }

        private void DropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as BatchIOConfigSectionViewModel).InputList.Add(e.Data.GetText());
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as BatchIOConfigSectionViewModel).InputList.AddRange(
                    e.Data.GetFileNames() ?? Array.Empty<string>());
        }

        private readonly Grid _inputFileGrid;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as BatchIOConfigSectionViewModel;
    }
}
