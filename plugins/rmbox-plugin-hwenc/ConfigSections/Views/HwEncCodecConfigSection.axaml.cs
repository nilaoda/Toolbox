using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.HwEnc.ConfigSections.HwEncCodecConfigSection",
        "编码")]
    public class HwEncCodecConfigSection : ConfigSectionBase
    {
        public HwEncCodecConfigSection()
        {
            InitializeComponent();
        }

        public HwEncCodecConfigSection(
            JToken sectionConfig)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as HwEncCodecConfigSectionViewModel;
    }
}
