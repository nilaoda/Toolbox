using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using ReactiveUI;
using Ruminoid.Common2.Metro.MetroControls;
using Ruminoid.Common2.Metro.MetroControls.Dialogs;
using Ruminoid.Toolbox.Composition.Services;
using Ruminoid.Toolbox.Shell.Services;
using Splat;

namespace Ruminoid.Toolbox.Shell.Windows
{
    [UsedImplicitly]
    public class SplashWindow : MetroWindow
    {
        public SplashWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public SplashWindowViewModel ViewModel => DataContext as SplashWindowViewModel;
    }

    [UsedImplicitly]
    public class SplashWindowViewModel : ReactiveObject
    {
        #region Constructor

        public SplashWindowViewModel()
        {
            Initialize =
                Observable.Create<object>(async observer =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.1));

                    InitializeStatus = "初始化插件...";
                    _ = Locator.Current.GetService<IPluginService>();

                    InitializeStatus = "初始化任务队列...";
                    _ = Locator.Current.GetService<QueueService>();

                    // Initialize MainWindow
                    InitializeStatus = "初始化 GUI...";
                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = new MainWindow();

                        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                            MessageBox.ShowAndGetResult(
                                "灾难性故障",
                                "发生了灾难性故障。请联系开发者反馈错误。\n" + ((e.ExceptionObject as Exception)?.Message ?? string.Empty),
                                desktop.MainWindow,
                                false);
                    }

                    observer.OnCompleted();

                    return Disposable.Empty;
                });
        }

        #endregion

        #region Initialize

        [UsedImplicitly]
        public readonly IObservable<object> Initialize;

        private string _initializeStatus = "启动初始化...";

        [UsedImplicitly]
        public string InitializeStatus
        {
            get => _initializeStatus;
            set => this.RaiseAndSetIfChanged(ref _initializeStatus, value);
        }

        #endregion

        #region Version

        [UsedImplicitly]
        public string VersionSummary { get; } = $"版本 {Assembly.GetExecutingAssembly().GetName().Version}";

        #endregion
    }
}
