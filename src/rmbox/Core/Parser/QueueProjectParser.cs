using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Ruminoid.Toolbox.Core.Parser
{
    [Export]
    public class QueueProjectParser : IParser
    {
        public QueueProjectParser(
            SingleProjectParser singleProjectParser,
            ILogger<QueueProjectParser> logger)
        {
            _singleProjectParser = singleProjectParser;

            _logger = logger;
        }

        public List<TaskCommand> Parse(JToken project)
        {
            try
            {
                _logger.LogDebug("Collecting operations.");
                JToken operations = project["operations"];

                if (operations is null)
                {
                    ArgumentNullException e = new ArgumentNullException(nameof(operations));
                    _logger.LogCritical(e, "Operations is null.");
                    throw e;
                }

                return operations
                    .SelectMany(x => _singleProjectParser.Parse(x))
                    .ToList();
            }
            catch (Exception e)
            {
                const string err = "解析项目文件时发生了错误。";
                _logger.LogCritical(e, err);
                throw new ProjectParseException(err, e);
            }
        }

        private readonly SingleProjectParser _singleProjectParser;

        private readonly ILogger<QueueProjectParser> _logger;
    }
}
