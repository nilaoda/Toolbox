using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Views;
using Splat;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class OperationsViewModel : ReactiveObject
    {
        public OperationsViewModel()
        {
            PluginHelper pluginHelper = Locator.Current.GetService<PluginHelper>();
            OperationsList = new List<OperationModel>(
                pluginHelper.OperationCollection
                    .Select(x =>
                        new OperationModel
                        {
                            Name = x.Item1.Name,
                            Description = x.Item1.Description,
                            Id = x.Item1.Id,
                            Type = x.Item2
                        }));

            // Initialize IsOperationSelected
            _isOperationSelected = this
                .WhenAnyValue(x => x.SelectedOperation)
                .Select(x => x is not null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.IsOperationSelected);
        }

        #region Data

        public List<OperationModel> OperationsList { get; }

        private OperationModel _selectedOperation;

        public OperationModel SelectedOperation
        {
            get => _selectedOperation;
            set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isOperationSelected;

        public bool IsOperationSelected => _isOperationSelected.Value;

        #endregion

        #region Commands

        public void CreateOperationWindow()
        {
            if (SelectedOperation is null) return;
            new OperationWindow(_selectedOperation).Show();
        }

        #endregion
    }
}
