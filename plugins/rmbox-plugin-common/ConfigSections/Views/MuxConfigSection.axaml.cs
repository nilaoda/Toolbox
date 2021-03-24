using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.MuxConfigSection",
        "混流")]
    public class MuxConfigSection : ConfigSectionBase
    {
        public MuxConfigSection()
        {
            InitializeComponent();
        }

        public MuxConfigSection(
            JToken sectionConfig)
        {
            DataContext = new MuxConfigSectionViewModel(this, sectionConfig);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as MuxConfigSectionViewModel;
    }
}
