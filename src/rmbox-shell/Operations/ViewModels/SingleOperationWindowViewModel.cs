using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Operations.Views;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.ViewModels.Project;
using Splat;

namespace Ruminoid.Toolbox.Shell.Operations.ViewModels
{
    public class SingleOperationWindowViewModel : ReactiveObject
    {
        public SingleOperationWindowViewModel(
            OperationModel operationModel,
            SingleOperationWindow window)
        {
            OperationModel = operationModel;
            _window = window;

            _pluginHelper = Locator.Current.GetService<PluginHelper>();
            _queueService = Locator.Current.GetService<QueueService>();

            InitializeTabs();
        }

        private readonly SingleOperationWindow _window;

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

            foreach (KeyValuePair<string, JToken> sectionData in _operation.RequiredConfigSections)
            {
                (ConfigSectionAttribute ConfigSectionAttribute, Type ConfigSectionType) sectionMeta = _pluginHelper.ConfigSectionCollection
                    .Where(x => x.ConfigSectionAttribute.Id == sectionData.Key)
                    .ToArray()
                    .FirstOrDefault();

                if (sectionMeta == default)
                    throw new ArgumentException("Cannot Find ConfigSection.");

                ConfigSectionBase section = Activator.CreateInstance(sectionMeta.ConfigSectionType, sectionData.Value) as ConfigSectionBase;

                if (section is null)
                    throw new ArgumentException("Cannot Construct ConfigSection.");

                _configSections.Add((sectionMeta.ConfigSectionAttribute, section));

                Items.Add(
                    new TabItem
                    {
                        Header = sectionMeta.ConfigSectionAttribute.Name,
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
            _window.ForceClose();
        }
        public void DoAddToQueueAndContinue() => _queueService.AddOrUpdate(GenerateProjectModel());

        public void DoExport()
        {
            throw new NotImplementedException();
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Utils

        private SingleProjectViewModel GenerateProjectModel() =>
            new(
                OperationModel,
                new Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)>(
                    _configSections.Select(
                            x => (x.ConfigSectionAttribute, x.ConfigSection.Config))
                        .ToList()));

        #endregion
    }
}
