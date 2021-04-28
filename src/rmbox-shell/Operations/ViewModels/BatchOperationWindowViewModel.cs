using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Operations.Views;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.Utils.ConfigSections;
using Ruminoid.Toolbox.Shell.ViewModels.Project;
using Splat;

namespace Ruminoid.Toolbox.Shell.Operations.ViewModels
{
    public class BatchOperationWindowViewModel : ReactiveObject
    {
        private readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)>
            _configSections = new();

        private readonly BatchOperationWindow _window;

        private IOperation _operation;

        public BatchOperationWindowViewModel(
            OperationModel operationModel,
            BatchOperationWindow window)
        {
            OperationModel = operationModel;
            _window = window;

            _pluginService = Locator.Current.GetService<IPluginService>();
            _queueService = Locator.Current.GetService<QueueService>();

            InitializeTabs();
        }

        private OperationModel OperationModel { get; }

        [UsedImplicitly]
        private Collection<TabItem> Items { get; } = new();

        #region Tabs

        private void InitializeTabs()
        {
#if !DEBUG

            try
            {

#endif

            ConfigSectionAttribute batchAttribute =
                Attribute.GetCustomAttribute(typeof(BatchIOConfigSection), typeof(ConfigSectionAttribute)) as
                    ConfigSectionAttribute;
            BatchIOConfigSection batchConfig = new BatchIOConfigSection();

            _configSections.Add((batchAttribute, batchConfig));

            Items.Add(
                new()
                {
                    Header = batchAttribute?.Name,
                    Content = batchConfig
                });

            _operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (_operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (var (key, _) in _operation.RequiredConfigSections)
            {
                // Exclude
                if (key == ConfigSectionBase.IOConfigSectionId)
                    continue;

                (ConfigSectionAttribute attribute, ConfigSectionBase section) =
                    _pluginService.CreateConfigSection(key);

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
                Observable.FromAsync(
                        () => MessageBox.ShowAndGetResult(
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

        #region Services

        private readonly IPluginService _pluginService;
        private readonly QueueService _queueService;

        #endregion

        #region Commands

        [UsedImplicitly]
        public void DoAddToQueue()
        {
            DoAddToQueueAndContinue();
            _window.ForceClose();
        }

        [UsedImplicitly]
        public void DoAddToQueueAndContinue() => _queueService.AddOrUpdate(GenerateProjectModel());

        [UsedImplicitly]
        public void DoExport()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utils

        private BatchProjectViewModel GenerateProjectModel() =>
            new(
                OperationModel,
                new Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)>(
                    _configSections.Select(
                            x => (x.ConfigSectionAttribute, x.ConfigSection.Config))
                        .ToList()));

        public static bool CheckCompatibilityAndReport(
            OperationModel operationModel,
            Window parent)
        {
            if (!(Activator.CreateInstance(operationModel.Type) is IOperation operation))
                throw new ArgumentException("Cannot Construct Operation.");

            if (operation.RequiredConfigSections.ContainsKey(ConfigSectionBase.IOConfigSectionId))
                return true;

            MessageBox.ShowAndGetResult(
                "不支持",
                $"{operationModel.Name} 不支持批量处理。",
                parent,
                false);

            return false;
        }

        #endregion
    }
}
