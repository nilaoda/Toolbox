using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264CoreConfigSection",
        "核心")]
    public class X264CoreConfigSection : ConfigSectionBase
    {
        public X264CoreConfigSection()
        {
            InitializeComponent();
        }

        public X264CoreConfigSection(
            JToken sectionConfig)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as X264CoreConfigSectionViewModel;
    }
}
