using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Ruminoid.Toolbox.Shell.Utils.Dialogs
{
    public class MessageBox : Window
    {
        #region Constructor

        public MessageBox()
        {
            throw new InvalidOperationException();
        }

        private MessageBox(
            string title,
            string content,
            bool showNoButton = true)
        {
            InitializeComponent();

            this.FindControl<TextBlock>("TitleBlock").Text = title;
            this.FindControl<TextBlock>("ContentBlock").Text = content;

            if (!showNoButton)
            {
                this.FindControl<Button>("YesButton").Content = "好";
                this.FindControl<Button>("NoButton").IsVisible = false;
            }

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion

        public static Task<bool> ShowAndGetResult(
            string title,
            string content,
            Window parent,
            bool showNoButton = true)
        {
            MessageBox window = new(title, content, showNoButton);
            return window.ShowDialog<bool>(parent);
        }

        private void YesClick(object sender, RoutedEventArgs e) => Close(true);

        private void NoClick(object sender, RoutedEventArgs e) => Close(false);
    }
}
