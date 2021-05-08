using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Utils.ConfigSections;
using Ruminoid.Toolbox.Shell.ViewModels.Project;

namespace Ruminoid.Toolbox.Shell.ViewModels.Operations
{
    public sealed class BatchOperationViewModel : OperationViewModelBase
    {
        public BatchOperationViewModel(OperationModel operationModel, UserControl view) : base(operationModel, view)
        {
#if !DEBUG

            try
            {

#endif

            ConfigSectionAttribute batchAttribute =
                Attribute.GetCustomAttribute(typeof(BatchIOConfigSection), typeof(ConfigSectionAttribute)) as
                    ConfigSectionAttribute;
            BatchIOConfigSection batchConfig = new();

            ConfigSections.Add((batchAttribute, batchConfig));

            Items.Add(
                new TabItem
                {
                    Header = batchAttribute?.Name,
                    Content = batchConfig
                });

            Operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (Operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (var (key, _) in Operation.RequiredConfigSections)
            {
                // Exclude
                if (key == ConfigSectionBase.IOConfigSectionId)
                    continue;

                (ConfigSectionAttribute attribute, ConfigSectionBase section) =
                    PluginService.CreateConfigSection(key);

                if (section is null)
                    throw new ArgumentException("Cannot Construct ConfigSection.");

                ConfigSections.Add((attribute, section));

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

        public override BatchProjectViewModel GenerateProjectModel() =>
            new(
                OperationModel,
                new Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)>(
                    ConfigSections.Select(
                            x => (x.ConfigSectionAttribute, x.ConfigSection.Config))
                        .ToList()));
    }
}
