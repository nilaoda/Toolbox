using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Audio.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Audio.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Audio.ConfigSections.AudioQualityConfigSection",
        "音频质量")]
    public class AudioQualityConfigSection : ConfigSectionBase
    {
        public AudioQualityConfigSection()
        {
            InitializeComponent();
        }

        public AudioQualityConfigSection(
            JToken sectionConfig)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as AudioQualityConfigSectionViewModel;
    }
}
