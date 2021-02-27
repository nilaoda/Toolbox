using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.X264.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.X264.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.X264.ConfigSections.X264EncodeQualityConfigSection",
        "视频质量")]
    public class X264EncodeQualityConfigSection : ConfigSectionBase
    {
        public X264EncodeQualityConfigSection()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region Data

        public override X264EncodeQualityConfigSectionViewModel Config => DataContext as X264EncodeQualityConfigSectionViewModel;

        #endregion
    }
}
