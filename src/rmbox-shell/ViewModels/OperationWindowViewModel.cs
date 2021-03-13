using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.Views;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class OperationWindowViewModel : ReactiveObject
    {
        public OperationWindowViewModel(
            OperationModel operationModel,
            OperationWindow window)
        {
            OperationModel = operationModel;
            _window = window;

            _pluginHelper = Locator.Current.GetService<PluginHelper>();
            _queueService = Locator.Current.GetService<QueueService>();

            InitializeTabs();
        }

        private readonly OperationWindow _window;

        private OperationModel OperationModel { get; }

        private Collection<TabItem> Items { get; } = new();

        #region Services

        private readonly PluginHelper _pluginHelper;
        private readonly QueueService _queueService;

        #endregion

        private IOperation _operation;

        private readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)>
            _configSections = new();

        #region Tabs

        private void InitializeTabs()
        {
            _operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (_operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (string sectionId in _operation.RequiredConfigSections)
            {
                (ConfigSectionAttribute ConfigSectionAttribute, Type ConfigSectionType) sectionData = _pluginHelper.ConfigSectionCollection
                    .Where(x => x.ConfigSectionAttribute.Id == sectionId)
                    .ToArray()
                    .FirstOrDefault();

                if (sectionData == default)
                    throw new ArgumentException("Cannot Find ConfigSection.");

                ConfigSectionBase section = Activator.CreateInstance(sectionData.ConfigSectionType) as ConfigSectionBase;

                if (section is null)
                    throw new ArgumentException("Cannot Construct ConfigSection.");

                _configSections.Add((sectionData.ConfigSectionAttribute, section));

                Items.Add(
                    new TabItem
                    {
                        Header = sectionData.ConfigSectionAttribute.Name,
                        Content = section
                    });
            }
        }

        #endregion

        #region Commands
        
        // ReSharper disable MemberCanBePrivate.Global

        public void DoAddToQueue()
        {
            DoAddToQueueAndContinue();
            _window.Close();
        }
        public void DoAddToQueueAndContinue() => _queueService.AddOrUpdate(GenerateProjectModel());

        public void DoExport()
        {
            throw new NotImplementedException();
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Utils

        private ProjectViewModel GenerateProjectModel() =>
            new(
                OperationModel,
                new Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)>(
                    _configSections.Select(
                            x => (x.ConfigSectionAttribute, x.ConfigSection.Config))
                        .ToList()));

        #endregion
    }
}
