using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections
{
    [ConfigSection("Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection")]
    public class IOConfigSection : ConfigSectionBase
    {
        public IOConfigSection()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public new string Header => "输入/输出";

        #region Data

        public new IOConfigSectionData Config { get; set; }

        #endregion
    }
}
