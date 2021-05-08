using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Core;
using Splat;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class PluginManagerView : UserControl
    {
        public PluginManagerView()
        {
            DataContext = new PluginManagerViewModel();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class PluginManagerViewModel : ReactiveObject
    {
        #region Constructor

        public PluginManagerViewModel()
        {
            IPluginService pluginService = Locator.Current.GetService<IPluginService>();
            PluginList = pluginService.MetaCollection
                .Select(x => x.Meta)
                .ToList();
        }

        #endregion

        #region Data

        [UsedImplicitly]
        public List<IMeta> PluginList { get; }

        private IMeta _selectedPlugin;

        [UsedImplicitly]
        public IMeta SelectedPlugin
        {
            get => _selectedPlugin;
            set => this.RaiseAndSetIfChanged(ref _selectedPlugin, value);
        }

        #endregion
    }
}
