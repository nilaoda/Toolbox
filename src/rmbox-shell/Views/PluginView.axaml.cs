using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Collections;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Services;
using Ruminoid.Toolbox.Shell.ViewModels.Operations;
using Ruminoid.Toolbox.Shell.Views.Operations;
using SearchSharp;
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
            _operationService = Locator.Current.GetService<OperationService>();

            OperationsCount = pluginService.OperationCollection.Count;

            OperationsList = new(
                pluginService.OperationCollection
                    .GroupBy(x => x.OperationAttribute.Category)
                    .Select(x =>
                    {
                        // Create OperationModel
                        ObservableCollectionEx<OperationModel> models = new(
                            x.Select(y => new OperationModel
                            {
                                Id = y.OperationAttribute.Id,
                                Name = y.OperationAttribute.Name,
                                Description = y.OperationAttribute.Description,
                                Rate = y.OperationAttribute.Rate,
                                Category = y.OperationAttribute.Category,
                                Author = y.OperationMeta.Author,
                                Type = y.OperationType
                            }));

                        // Create SearchStorage
                        SearchStorage<OperationModel> searchStorage = new()
                        {
                            Mode = CharParseMode.EnablePinyinSearch
                        };
                        searchStorage.Add(models, y => y.Name);

                        return new OperationModel
                        {
                            Name = x.Key,
                            Children = models,
                            SearchStorage = searchStorage
                        };
                    }));

            this
                .ObservableForProperty(x => x.OperationSearchText)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => OperationsList
                    .ForEach(y => y.Children.Reset(
                        y.SearchStorage.Search(x.Value))));

            this
                .ObservableForProperty(x => x.OperationSearchText)
                .Subscribe(_ => ExpendTree());

            _isOperationSelected = this
                .WhenAnyValue(x => x.SelectedOperation)
                .Select(x => x?.Type != null)
                .ToProperty(this, x => x.IsOperationSelected);
        }

        #region View

        private readonly PluginView _view;

        private TreeView _pluginTreeView;

        private TreeView PluginTreeView => _pluginTreeView ??= _view.Get<TreeView>("PluginTreeView");

        #endregion

        #region Data

        [UsedImplicitly]
        public int OperationsCount { get; }

        public ObservableCollectionEx<OperationModel> OperationsList { get; }

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

        private readonly OperationService _operationService;

        #region Commands

        [UsedImplicitly]
        public void ExpendTree(bool expend = true)
        {
            if (PluginTreeView.Presenter?.Panel?.Children is null)
                return;

            foreach (IControl child in PluginTreeView.Presenter.Panel.Children)
                if (child is TreeViewItem item)
                    item.IsExpanded = expend;
        }

        [UsedImplicitly]
        public void DoCreateNewOperation()
        {
            if (SelectedOperation is null) return;
            _operationService.AddOperation(
                SelectedOperation.Name,
                new SingleOperationView(SelectedOperation));
        }

        [UsedImplicitly]
        public void DoCreateNewBatch()
        {
            if (SelectedOperation is null) return;
            if (!OperationViewModelBase.CheckCompatibilityAndReport(SelectedOperation, _view.GetVisualRoot() as Window)) return;
            _operationService.AddOperation(
                $"批量：{SelectedOperation.Name}",
                new BatchOperationView(SelectedOperation));
        }

        #endregion
    }
}
