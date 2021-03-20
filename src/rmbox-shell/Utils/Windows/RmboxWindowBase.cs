using Avalonia.Controls;

namespace Ruminoid.Toolbox.Shell.Utils.Windows
{
    public abstract class RmboxWindowBase : Window
    {
        #region Close Confirm

        protected bool CloseConfirmed;

        public void ForceClose(object dialogResult)
        {
            CloseConfirmed = true;
            Close(dialogResult);
        }

        #endregion
    }
}
