using System;
using System.Composition;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ruminoid.Toolbox.Formatting
{
    [Export]
    public class FormattingHelper
    {
        public FormattingHelper()
        {
            FormatData = ReceiveData
                .Select(Format)
                .Where(x => x is not null);
        }

        #region Target

        private string _currentTarget = "";

        public void UpdateTarget(string target) => _currentTarget = target;

        #endregion

        #region Subjects

        public Subject<Tuple<string, string>> ReceiveData = new();

        public IObservable<FormattedEvent> FormatData;

        #endregion

        private FormattedEvent Format(Tuple<string, string> arg)
        {
            throw new NotImplementedException();
        }
    }
}
