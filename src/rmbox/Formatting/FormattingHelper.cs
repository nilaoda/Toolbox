using System;
using System.Collections.Generic;
using System.Composition;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Ruminoid.Toolbox.Composition;
using Ruminoid.Toolbox.Helpers.CommandLine;

namespace Ruminoid.Toolbox.Formatting
{
    [Export]
    public class FormattingHelper
    {
        public FormattingHelper(
            CommandLineHelper commandLineHelper,
            PluginHelper pluginHelper,
            ILogger<FormattingHelper> logger)
        {
            _commandLineHelper = commandLineHelper;
            _pluginHelper = pluginHelper;
            _logger = logger;

            FormatData = ReceiveData
                .Select(Format)
                .Where(x => x is not null);
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
            var (_, formatter) = _pluginHelper.GetFormatter(formatTarget);
            return formatter.Format(formatTarget, data, sessionStorage);
        }
        
        private readonly CommandLineHelper _commandLineHelper;
        private readonly PluginHelper _pluginHelper;
        private readonly ILogger<FormattingHelper> _logger;
    }
}
