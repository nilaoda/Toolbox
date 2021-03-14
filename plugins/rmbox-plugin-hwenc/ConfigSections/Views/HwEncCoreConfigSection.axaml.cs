using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCoreConfigSection",
        "核心")]
    public class HwEncCoreConfigSection : ConfigSectionBase
    {
        public HwEncCoreConfigSection()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as HwEncCoreConfigSectionViewModel;
    }
}
