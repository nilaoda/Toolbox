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
    public class ChooseOperationWindowViewModel : ReactiveObject
    {
        public ChooseOperationWindowViewModel(
            ChooseOperationWindow window)
        {
            _window = window;

            PluginHelper pluginHelper = Locator.Current.GetService<PluginHelper>();
            OperationsList = new List<OperationModel>(
                pluginHelper.OperationCollection
                    .Select(x =>
                        new OperationModel
                        {
                            Name = x.OperationAttribute.Name,
                            Description = x.OperationAttribute.Description,
                            Id = x.OperationAttribute.Id,
                            Type = x.OperationType
                        }));

            // Initialize IsOperationSelected
            _isOperationSelected = this
                .WhenAnyValue(x => x.SelectedOperation)
                .Select(x => x is not null)
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

        private readonly ChooseOperationWindow _window;

        #region Commands

        public void DoConfirm()
        {
            if (SelectedOperation is null) return;
            _window.Close(SelectedOperation);
        }

        public void DoCancel()
        {
            _window.Close();
        }

        #endregion
    }
}
