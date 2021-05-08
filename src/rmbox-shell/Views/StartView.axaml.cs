using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Windows;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class StartView : UserControl
    {
        public StartView()
        {
            DataContext = new StartViewModel(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class StartViewModel : ReactiveObject
    {
        public StartViewModel(
            StartView view)
        {
            _view = view;
        }

        private readonly StartView _view;
        private MainWindow _window;

        private MainWindow Window => _window ??= (MainWindow) _view.GetVisualRoot();

        #region Commands

        [UsedImplicitly]
        public void DoCreateNewOperation() =>
            // ReSharper disable once PossibleNullReferenceException
            (Window.DataContext as MainWindowViewModel).CurrentTabIndex = (int) CommonTabIndex.PluginsView;

        [UsedImplicitly]
        public void DoShowAboutWindow() =>
            // ReSharper disable once PossibleNullReferenceException
            (Window.DataContext as MainWindowViewModel).CurrentTabIndex = (int)CommonTabIndex.AboutView;

        [UsedImplicitly]
        public void DoClose() => Window.Close();

        #endregion
    }
}
