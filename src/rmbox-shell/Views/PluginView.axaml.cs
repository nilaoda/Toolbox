using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Utils.UserTypes;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Shell.Models;
using Splat;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class PluginView : UserControl
    {
        public PluginView()
        {
            DataContext = new PluginViewModel(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class PluginViewModel : ReactiveObject
    {
        public PluginViewModel(
            PluginView view)
        {
            _view = view;

            IPluginService pluginService = Locator.Current.GetService<IPluginService>();

            OperationsCount = pluginService.OperationCollection.Count;

            OperationsList = pluginService.OperationCollection
                .GroupBy(x => x.OperationAttribute.Category)
                .Select(x => new OperationModel
                {
                    Name = x.Key,
                    Children = x.Select(y => new OperationModel
                    {
                        Id = y.OperationAttribute.Id,
                        Name = y.OperationAttribute.Name,
                        Description = y.OperationAttribute.Description,
                        Rate = y.OperationAttribute.Rate,
                        Category = y.OperationAttribute.Category,
                        Author = y.OperationMeta.Author,
                        Type = y.OperationType
                    }).ToList()
                })
                .ToList();

            // Initialize IsOperationSelected
            _isOperationSelected = this
                .WhenAnyValue(x => x.SelectedOperation)
                .Select(x => x?.Type != null)
                .ToProperty(this, x => x.IsOperationSelected);
        }

        private readonly PluginView _view;

        #region Data

        [UsedImplicitly]
        public int OperationsCount { get; }

        [UsedImplicitly]
        public List<OperationModel> OperationsList { get; }

        private OperationModel _selectedOperation;

        [UsedImplicitly]
        public OperationModel SelectedOperation
        {
            get => _selectedOperation;
            set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isOperationSelected;

        [UsedImplicitly]
        public bool IsOperationSelected => _isOperationSelected.Value;

        #endregion
    }
}
