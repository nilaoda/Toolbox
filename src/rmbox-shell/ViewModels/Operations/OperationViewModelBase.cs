using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.ViewModels.Project;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels.Operations
{
    public interface IProjectModelGenerator
    {
        public ProjectViewModel GenerateProjectModel();
    }

    public abstract class OperationViewModelBase : ReactiveObject, IProjectModelGenerator
    {
        protected OperationViewModelBase(
            OperationModel operationModel,
            UserControl view)
        {
            _view = view;
            OperationModel = operationModel;

            PluginService = Locator.Current.GetService<IPluginService>();
            _queueService = Locator.Current.GetService<QueueService>();
        }

        protected IOperation Operation;

        protected OperationModel OperationModel { get; }

        [UsedImplicitly]
        public Collection<TabItem> Items { get; } = new();

        protected readonly Collection<(ConfigSectionAttribute ConfigSectionAttribute, ConfigSectionBase ConfigSection)>
            ConfigSections = new();

        #region Services

        protected readonly IPluginService PluginService;
        private readonly QueueService _queueService;

        #endregion

        private readonly UserControl _view;

        #region Commands

        [UsedImplicitly]
        public void DoAddToQueue()
        {
            DoAddToQueueAndContinue();
            Locator.Current.GetService<OperationService>().CloseOperation(_view);
        }

        [UsedImplicitly]
        public void DoAddToQueueAndContinue() => _queueService.AddOrUpdate(GenerateProjectModel());

        [UsedImplicitly]
        public void DoExport()
        {
            throw new NotImplementedException();
        }

        #endregion

        public abstract ProjectViewModel GenerateProjectModel();

        #region Utils

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
