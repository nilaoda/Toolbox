using System;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        IntroConfigSectionId,
        "介绍")]
    public class IntroConfigSection : ConfigSectionBase
    {
        public IntroConfigSection()
        {
            throw new InvalidOperationException();
        }

        public IntroConfigSection(
            JToken sectionConfig)
        {
            DataContext = new IntroConfigSectionViewModel(sectionConfig);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as IntroConfigSectionViewModel;
    }
}
