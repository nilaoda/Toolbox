using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncQualityConfigSection",
        "视频质量")]
    public class HwEncQualityConfigSection : ConfigSectionBase
    {
        public HwEncQualityConfigSection()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as HwEncQualityConfigSectionViewModel;
    }
}
