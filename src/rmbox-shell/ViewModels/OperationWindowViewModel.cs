using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class OperationWindowViewModel : ReactiveObject
    {
        public OperationWindowViewModel(
            OperationModel operationModel)
        {
            OperationModel = operationModel;

            _pluginHelper = Locator.Current.GetService<PluginHelper>();

            InitializeTabs();
        }

        public OperationModel OperationModel { get; }

        public Collection<TabItem> Items { get; } = new();

        private readonly PluginHelper _pluginHelper;

        private IOperation _operation;
        private Collection<Tuple<ConfigSectionAttribute, ConfigSectionBase>> _configSections = new();

        #region Tabs

        private void InitializeTabs()
        {
            _operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (_operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (string sectionId in _operation.RequiredConfigSections)
            {
                Tuple<ConfigSectionAttribute, Type> sectionData = _pluginHelper.ConfigSectionCollection
                    .Where(x => x.Item1.Id == sectionId)
                    .ToArray()
                    .FirstOrDefault();

                if (sectionData is null)
                    throw new ArgumentException("Cannot Find ConfigSection.");

                ConfigSectionBase section = Activator.CreateInstance(sectionData.Item2) as ConfigSectionBase;

                if (section is null)
                    throw new ArgumentException("Cannot Construct ConfigSection.");

                _configSections.Add(
                    new Tuple<ConfigSectionAttribute, ConfigSectionBase>(
                        sectionData.Item1,
                        section));

                Items.Add(
                    new TabItem
                    {
                        Header = sectionData.Item1.Name,
                        Content = section
                    });
            }
        }

        #endregion
    }
}
