using System;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.AdvancedTrackConfigSection",
        "高级 (流)")]
    public class AdvancedTrackConfigSection : ConfigSectionBase
    {
        public AdvancedTrackConfigSection()
        {
            throw new InvalidOperationException();
        }

        public AdvancedTrackConfigSection(
            JToken sectionConfig)
        {
            DataContext = new AdvancedTrackConfigSectionViewModel();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as AdvancedTrackConfigSectionViewModel;
    }
}
