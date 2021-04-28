using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Composition.Services;
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

            _pluginService = Locator.Current.GetService<IPluginService>();
            _queueService = Locator.Current.GetService<QueueService>();

            InitializeTabs();
        }

        private readonly SingleOperationWindow _window;

        private OperationModel OperationModel { get; }

        private Collection<TabItem> Items { get; } = new();

        #region Services

        private readonly IPluginService _pluginService;
        private readonly QueueService _queueService;

        #endregion

        private IOperation _operation;

        private readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)>
            _configSections = new();

        #region Tabs

        private void InitializeTabs()
        {
#if !DEBUG

            try
            {

#endif

            _operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (_operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (KeyValuePair<string, JToken> sectionData in _operation.RequiredConfigSections)
            {
                (ConfigSectionAttribute attribute, ConfigSectionBase section) = _pluginService.CreateConfigSection(sectionData.Key);

                if (section is null)
                    throw new ArgumentException("Cannot Construct ConfigSection.");

                _configSections.Add((attribute, section));

                Items.Add(
                    new TabItem
                    {
                        Header = attribute.Name,
                        Content = section
                    });
            }

#if !DEBUG

            }
            catch (Exception)
            {
                System.Reactive.Linq.Observable.FromAsync(
                        () => Common2.Metro.MetroControls.Dialogs.MessageBox.ShowAndGetResult(
                            "警告",
                            "插件加载出现错误，请检查是否安装了所需的插件。操作可能出现不正常的行为。",
                            _window,
                            false),
                        RxApp.MainThreadScheduler)
                    .Subscribe(_ => { });
            }

#endif

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
