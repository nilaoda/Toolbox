using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Audio.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Audio.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Audio.AudioConfigSection",
        "音频")]
    public class AudioConfigSection : ConfigSectionBase
    {
        public AudioConfigSection()
        {
            InitializeComponent();
        }

        public AudioConfigSection(
            JToken sectionConfig)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as AudioConfigSectionViewModel;
    }
}
