using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Ruminoid.Toolbox.Composition.Services;

namespace Ruminoid.Toolbox.Formatting
{
    public class FormattingService
    {
        public FormattingService(
            IPluginService pluginService)
        {
            _pluginService = pluginService;

            FormatData = ReceiveData
                .Select(Format)
                .WhereNotNull();
        }
        
        #region Subjects

        public readonly Subject<(string FormatTarget, string Data, Dictionary<string, object> SessionStorage)>
            ReceiveData = new();

        public readonly IObservable<FormattedEvent> FormatData;

        #endregion

        private FormattedEvent Format(
            (string FormatTarget, string Data, Dictionary<string, object> SessionStorage) arg)
        {
            var (formatTarget, data, sessionStorage) = arg;
            (_, IFormatter formatter) = _pluginService.GetFormatter(formatTarget);
            return formatter.Format(formatTarget, data, sessionStorage);
        }

        private readonly IPluginService _pluginService;
    }
}
