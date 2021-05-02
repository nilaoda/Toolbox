using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Utils.Text;
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

            _displayOperationsList = this
                .WhenAnyValue(x => x.OperationSearchText)
                .Select(x => OperationsList
                    .Select(y =>
                        new OperationModel
                        {
                            Name = y.Name,
                            Children = SearchUtils.Search(
                                y.Children,
                                z => z.Name,
                                x)
                        })
                    .Where(y => y.Children is not null && y.Children.Any())
                    .ToList())
                .ToProperty(this, x => x.DisplayOperationsList);

            _isOperationSelected = this
                .WhenAnyValue(x => x.SelectedOperation)
                .Select(x => x?.Type != null)
                .ToProperty(this, x => x.IsOperationSelected);

            this
                .ObservableForProperty(x => x.DisplayOperationsList)
                .Subscribe(_ => ExpendTree());
        }

        #region View

        private readonly PluginView _view;

        private TreeView _pluginTreeView;

        private TreeView PluginTreeView => _pluginTreeView ??= _view.Get<TreeView>("PluginTreeView");

        #endregion

        #region Data

        [UsedImplicitly]
        public int OperationsCount { get; }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable InconsistentNaming
        private readonly List<OperationModel> OperationsList;

        private readonly ObservableAsPropertyHelper<List<OperationModel>> _displayOperationsList;

        [UsedImplicitly]
        public List<OperationModel> DisplayOperationsList => _displayOperationsList.Value;

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

        private string _operationSearchText = "";

        [UsedImplicitly]
        public string OperationSearchText
        {
            get => _operationSearchText;
            set => this.RaiseAndSetIfChanged(ref _operationSearchText, value);
        }

        #endregion

        #region Commands

        [UsedImplicitly]
        public void ExpendTree(bool expend = true)
        {
            foreach (IControl child in PluginTreeView.Presenter.Panel.Children)
                if (child is TreeViewItem item)
                    item.IsExpanded = expend;
        }

        #endregion
    }
}
