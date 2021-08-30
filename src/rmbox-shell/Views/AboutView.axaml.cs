using System.Reflection;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class AboutView : UserControl
    {
        public AboutView()
        {
            DataContext = new AboutViewModel();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class AboutViewModel : ReactiveObject
    {
        [UsedImplicitly]
        public string VersionSummary { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        [UsedImplicitly]
        public string VersionDetail { get; } = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        [UsedImplicitly]
        public string RuntimeInformation { get; } =
            $"RuntimeIdentifier: {System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier}\nFrameworkDescription: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\nProcessArchitecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}\nOSArchitecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}\nOSDescription: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}\n";

        [UsedImplicitly]
        public string ReleaseNoteMarkdown { get; } =
            typeof(AboutView).Assembly
                .GetManifestResourceStream(
                    "Ruminoid.Toolbox.Shell.Resources.Markdowns.ReleaseNote.md")
                .ReadStreamToEnd();
    }
}
