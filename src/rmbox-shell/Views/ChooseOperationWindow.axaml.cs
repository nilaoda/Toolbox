using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ruminoid.Toolbox.Shell.Models;
using Ruminoid.Toolbox.Shell.ViewModels;

namespace Ruminoid.Toolbox.Shell.Views
{
    public class ChooseOperationWindow : Window
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public ChooseOperationWindow()
        {
            DataContext = new ChooseOperationWindowViewModel(this);

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static Task<OperationModel> ChooseOperation(
            Window owner)
        {
            ChooseOperationWindow window = new ChooseOperationWindow();
            return window.ShowDialog<OperationModel>(owner);
        }
    }
}
