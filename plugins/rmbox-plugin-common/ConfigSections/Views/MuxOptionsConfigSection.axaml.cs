using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxOptionsConfigSection",
        "混流选项")]
    public class MuxOptionsConfigSection : ConfigSectionBase
    {
        public MuxOptionsConfigSection()
        {
            InitializeComponent();
        }

        public MuxOptionsConfigSection(
            JToken sectionConfig)
        {
            DataContext = new MuxOptionsConfigSectionViewModel();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as MuxOptionsConfigSectionViewModel;
    }
}
