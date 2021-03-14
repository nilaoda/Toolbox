using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.CustomArgsConfigSection",
        "自定义参数")]
    public class CustomArgsConfigSection : ConfigSectionBase
    {
        public CustomArgsConfigSection()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override object Config => DataContext as CustomArgsConfigSectionViewModel;
    }
}
