using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        IOConfigSectionId,
        "输入/输出")]
    public sealed class IOConfigSection : ConfigSectionBase
    {
        public IOConfigSection()
        {
            DataContext = new IOConfigSectionViewModel(this, null);

            InitializeComponent();
        }

        public IOConfigSection(
            JToken sectionConfig)
        {
            DataContext = new IOConfigSectionViewModel(this, sectionConfig);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #region Data

        public override IOConfigSectionViewModel Config => DataContext as IOConfigSectionViewModel;

        #endregion
    }
}
