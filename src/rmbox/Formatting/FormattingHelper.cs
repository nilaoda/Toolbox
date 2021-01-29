using System.Composition;

namespace Ruminoid.Toolbox.Formatting
{
    [Export]
    public class FormattingHelper
    {
        public FormattingHelper()
        {
        }

        #region Target

        private string _currentTarget = "";

        public void UpdateTarget(string target) => _currentTarget = target;

        #endregion
    }
}
