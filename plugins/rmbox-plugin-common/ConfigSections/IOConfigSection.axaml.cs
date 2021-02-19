using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Core;

namespace Ruminoid.Toolbox.Plugins.Common.ConfigSections
{
    [ConfigSection(
        "Ruminoid.Toolbox.Plugins.Common.ConfigSections.IOConfigSection",
        "输入/输出")]
    public sealed class IOConfigSection : ConfigSectionBase
    {
        public IOConfigSection()
        {
            DataContext = Config;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #region Data

        public override IOConfigSectionData Config { get; } = new();

        #endregion
    }
}
