using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Views.Views
{
    public class QueueView : UserControl
    {
        public QueueView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
