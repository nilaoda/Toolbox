using System.Reflection;
using ReactiveUI;
using Ruminoid.Toolbox.Shell.Views;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class AboutWindowViewModel : ReactiveObject
    {
        public AboutWindowViewModel(
            AboutWindow window)
        {
            _window = window;
        }

        private readonly AboutWindow _window;

        public string VersionSummary { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        public string VersionDetail { get; } = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        #region Commands

        public void DoCloseWindow()
        {
            _window.Close();
        }

        #endregion
    }
}
