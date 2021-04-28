using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Core;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
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

        public List<IMeta> PluginList { get; }

        private IMeta _selectedPlugin;

        public IMeta SelectedPlugin
        {
            get => _selectedPlugin;
            set => this.RaiseAndSetIfChanged(ref _selectedPlugin, value);
        }

        #endregion
    }
}
