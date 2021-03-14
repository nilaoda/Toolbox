using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class QueueView : UserControl
    {
        public QueueView()
        {
            DataContext = new QueueViewModel(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
