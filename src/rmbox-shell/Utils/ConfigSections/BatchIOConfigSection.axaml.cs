using Avalonia.Markup.Xaml;
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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as BatchIOConfigSectionViewModel;
    }
}
