using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Plugins.Common.ConfigSections.ViewModels;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections.Views
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection",
        "输入/输出")]
    public sealed class IOConfigSection : ConfigSectionBase
    {
        public IOConfigSection()
        {
            DataContext = new IOConfigSectionViewModel(this);

            InitializeComponent();
        }

        public IOConfigSection(
            JToken sectionConfig)
        {
            DataContext = new IOConfigSectionViewModel(this);

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
