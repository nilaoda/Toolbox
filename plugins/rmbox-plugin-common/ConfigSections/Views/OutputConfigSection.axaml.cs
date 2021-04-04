using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.OutputConfigSection",
        "输出")]
    public class OutputConfigSection : ConfigSectionBase
    {
        public OutputConfigSection()
        {
            InitializeComponent();
        }

        public OutputConfigSection(
            JToken sectionConfig)
        {
            DataContext = new OutputConfigSectionViewModel(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as OutputConfigSectionViewModel;
    }
}
