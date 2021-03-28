using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Utils.Extensions;

namespace Ruminoid.Toolbox.Core.Parser
{
    [Export]
    public class BatchProjectParser : IParser
    {
        #region Constructor

        public BatchProjectParser(
            QueueProjectParser queueProjectParser,
            ILogger<BatchProjectParser> logger)
        {
            _queueProjectParser = queueProjectParser;
            _logger = logger;
        }

        #endregion

        public List<TaskCommand> Parse(JToken project)
        {
            string subtitleFormat = project["subtitle_format"]?.ToObject<string>();
            string outputFormat = project["output_format"]?.ToObject<string>();
            string operation = project["operation"]?.ToObject<string>();
            
            if (string.IsNullOrWhiteSpace(outputFormat))
            {
                ArgumentNullException e = new(nameof(outputFormat));
                _logger.LogCritical(e, "The output format is null.");
                throw e;
            }

            if (operation is null)
            {
                ArgumentNullException e = new ArgumentNullException(nameof(operation));
                _logger.LogCritical(e, "Operation is null.");
                throw e;
            }

            _logger.LogDebug("Collecting sections.");
            List<JToken> sections = project["sections"].ToObject<List<JToken>>();

            if (sections is null)
            {
                ArgumentNullException e = new(nameof(sections));
                _logger.LogCritical(e, "The sections field is null.");
                throw e;
            }

            _logger.LogDebug("Collecting inputs.");
            List<string> inputs = project["inputs"].ToObject<List<string>>();

            if (inputs is null)
            {
                ArgumentNullException e = new(nameof(inputs));
                _logger.LogCritical(e, "The inputs field is null.");
                throw e;
            }

            return _queueProjectParser.Parse(JObject.FromObject(new
            {
                operations = (IEnumerable<JToken>) inputs
                    .Select(x => new
                    {
                        operation,
                        sections = new List<JToken>(sections)
                        {
                            JObject.FromObject(new
                            {
                                type = ConfigSectionBase.IOConfigSectionId,
                                data = new
                                {
                                    input = x,
                                    subtitle = x.FormatPath(subtitleFormat ?? ""),
                                    output = x.FormatPath(outputFormat),
                                    use_vsfm = project["use_vsfm"].ToObject<bool>()
                                }
                            })
                        }
                    })
                    .Select(JObject.FromObject)
            }));
        }

        private readonly QueueProjectParser _queueProjectParser;

        private readonly ILogger<BatchProjectParser> _logger;
    }
}
