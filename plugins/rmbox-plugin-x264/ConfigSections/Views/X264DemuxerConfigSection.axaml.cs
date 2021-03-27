using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264DemuxerConfigSection",
        "分离器")]
    public class X264DemuxerConfigSection : ConfigSectionBase
    {
        public X264DemuxerConfigSection()
        {
            InitializeComponent();
        }

        public X264DemuxerConfigSection(
            JToken sectionConfig)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as X264DemuxerConfigSectionViewModel;
    }
}
