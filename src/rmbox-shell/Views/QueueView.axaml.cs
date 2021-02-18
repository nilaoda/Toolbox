using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.ViewModels;
using Splat;

namespace Ruminoid.Toolbox.Shell.Views.Views
{
    public class QueueView : UserControl
    {
        public QueueView()
        {
            DataContext = Locator.Current.GetService<QueueViewModel>();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
