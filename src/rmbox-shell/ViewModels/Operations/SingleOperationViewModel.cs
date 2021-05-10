// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Core;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.ViewModels.Project;

namespace Ruminoid.Toolbox.Shell.ViewModels.Operations
{
    public sealed class SingleOperationViewModel : OperationViewModelBase
    {
        public SingleOperationViewModel(OperationModel operationModel, UserControl view) : base(operationModel, view)
        {
#if !DEBUG

            try
            {

#endif

            Operation = Activator.CreateInstance(OperationModel.Type) as IOperation;

            if (Operation is null)
                throw new ArgumentException("Cannot Construct Operation.");

            foreach (KeyValuePair<string, JToken> sectionData in Operation.RequiredConfigSections)
            {
                (ConfigSectionAttribute attribute, ConfigSectionBase section) =
                    PluginService.CreateConfigSection(sectionData.Key, sectionData.Value);

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
                System.Reactive.Linq.Observable.FromAsync(
                        () => MessageBox.ShowAndGetResult(
                            "警告",
                            "插件加载出现错误，请检查是否安装了所需的插件。操作可能出现不正常的行为。",
                            view.GetVisualRoot() as Window,
                            false),
                        RxApp.MainThreadScheduler)
                    .Subscribe(_ => { });
            }

#endif
        }

        public override SingleProjectViewModel GenerateProjectModel() =>
            new(
                OperationModel,
                new Collection<(ConfigSectionAttribute ConfigSectionAttribute, object ConfigSection)>(
                    ConfigSections.Select(
                            x => (x.ConfigSectionAttribute, x.ConfigSection.Config))
                        .ToList()));
    }
}
