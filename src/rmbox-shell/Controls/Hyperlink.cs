// https://github.com/AvaloniaUtils/MessageBox.Avalonia/blob/master/src/MessageBox.Avalonia/Controls/Hyperlink.cs

using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Ruminoid.Toolbox.Shell.Controls
{
    public class Hyperlink : TextBlock
    {
        private string _url;

        public static readonly DirectProperty<Hyperlink, string> UrlProperty
            = AvaloniaProperty.RegisterDirect<Hyperlink, string>(nameof(Url), o => o.Url, (o, v) => o.Url = v);

        public string Url
        {
            get => _url;
            set => SetAndRaise(UrlProperty, ref _url, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (string.IsNullOrEmpty(Url)) return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                new Process {StartInfo = {UseShellExecute = true, FileName = Url}}.Start(); // https://stackoverflow.com/a/2796367/241446
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("x-www-browser", Url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Process.Start("open", Url);
        }

    }
}
