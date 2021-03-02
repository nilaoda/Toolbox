using System;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.Views;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(
            MainWindow window)
        {
            _window = window;
        }

        private readonly MainWindow _window;

        #region Commands

        public async void DoCreateNewOperation()
        {
            OperationModel result = await ChooseOperationWindow.ChooseOperation(_window);
            if (result is null) return;
            new OperationWindow(result).Show(_window);
        }

        public void DoCreateNewChain()
        {
            throw new NotImplementedException();
        }

        public void DoShowAboutWindow()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
