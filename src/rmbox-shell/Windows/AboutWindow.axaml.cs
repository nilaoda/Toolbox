using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls;

namespace Ruminoid.Toolbox.Shell.Windows
{
    public class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            DataContext = new AboutWindowViewModel(this);

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class AboutWindowViewModel : ReactiveObject
    {
        public AboutWindowViewModel(
            AboutWindow window)
        {
            _window = window;
        }

        private readonly AboutWindow _window;

        [UsedImplicitly]
        public string VersionSummary { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        [UsedImplicitly]
        public string VersionDetail { get; } = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        #region Commands

        [UsedImplicitly]
        public void DoCloseWindow()
        {
            _window.Close();
        }

        #endregion
    }
}
