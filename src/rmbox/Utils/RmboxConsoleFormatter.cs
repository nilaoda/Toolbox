using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Ruminoid.Toolbox.Utils
{
    public class RmboxConsoleFormatter : ConsoleFormatter, IDisposable
    {
        public const string RmboxConsoleFormatterName = "rmbox";
        private const string LogLevelPadding = ": ";
        private static readonly string MessagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length);
        private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;

        public RmboxConsoleFormatter()
            : base(RmboxConsoleFormatterName)
        {
        }
        
        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
                return;

            LogLevel logLevel = logEntry.LogLevel;
            ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
            string logLevelString = GetLogLevelString(logLevel);

            const string timestampFormat = "yy-MM-dd hh:mm:ss ";
            DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
            var timestamp = dateTimeOffset.ToString(timestampFormat);

            textWriter.Write(timestamp);

            if (logLevelString != null)
                WriteColoredMessage(textWriter, logLevelString, logLevelColors.Background, logLevelColors.Foreground);

            CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
        }

        private static void WriteColoredMessage(TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
            if (background.HasValue) textWriter.Write(AnsiParser.GetBackgroundColorEscapeCode(background.Value));
            if (foreground.HasValue) textWriter.Write(AnsiParser.GetForegroundColorEscapeCode(foreground.Value));
            textWriter.Write(message);
            if (foreground.HasValue)
                textWriter.Write(AnsiParser.DefaultForegroundColor); // reset to default foreground color
            if (background.HasValue)
                textWriter.Write(AnsiParser.DefaultBackgroundColor); // reset to the background color
        }

        private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string message, IExternalScopeProvider scopeProvider)
        {
            Exception exception = logEntry.Exception;
            
            textWriter.Write(LogLevelPadding);
            textWriter.Write(logEntry.Category.Split('.').LastOrDefault());
            
            WriteScopeInformation(textWriter, scopeProvider);
            WriteMessage(textWriter, message);

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null) WriteMessage(textWriter, exception.ToString());
        }

        private void WriteMessage(TextWriter textWriter, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                textWriter.Write(MessagePadding);
                WriteReplacing(textWriter, Environment.NewLine, NewLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }

            static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
            {
                string newMessage = message.Replace(oldValue, newValue);
                writer.Write(newMessage);
            }
        }

        private static string GetLogLevelString(LogLevel logLevel) =>
            logLevel switch
            {
                // ReSharper disable StringLiteralTypo
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                // ReSharper restore StringLiteralTypo
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel) =>
            logLevel switch
            {
                LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
                LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
                LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new ConsoleColors(null, null)
            };

        private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider) =>
            scopeProvider?.ForEachScope((scope, state) =>
            {
                state.Write(" => ");
                state.Write(scope);
            }, textWriter);

        private readonly struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }

        public void Dispose()
        {
        }
    }

    internal class AnsiParser
    {
        private readonly Action<string, int, int, ConsoleColor?, ConsoleColor?> _onParseWrite;
        public AnsiParser(Action<string, int, int, ConsoleColor?, ConsoleColor?> onParseWrite) =>
            _onParseWrite = onParseWrite ?? throw new ArgumentNullException(nameof(onParseWrite));
        
        public void Parse(string message)
        {
            int startIndex = -1;
            int length = 0;
            ConsoleColor? foreground = null;
            ConsoleColor? background = null;
            var span = message.AsSpan();
            const char EscapeChar = '\x1B';
            bool isBright = false;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == EscapeChar && span.Length >= i + 4 && span[i + 1] == '[')
                {
                    int escapeCode;
                    if (span[i + 3] == 'm')
                    {
                        // Example: \x1B[1m
                        if (IsDigit(span[i + 2]))
                        {
                            escapeCode = (int)(span[i + 2] - '0');
                            if (startIndex != -1)
                            {
                                _onParseWrite(message, startIndex, length, background, foreground);
                                startIndex = -1;
                                length = 0;
                            }
                            if (escapeCode == 1)
                                isBright = true;
                            i += 3;
                            continue;
                        }
                    }
                    else if (span.Length >= i + 5 && span[i + 4] == 'm')
                    {
                        // Example: \x1B[40m
                        if (IsDigit(span[i + 2]) && IsDigit(span[i + 3]))
                        {
                            escapeCode = (int)(span[i + 2] - '0') * 10 + (int)(span[i + 3] - '0');
                            if (startIndex != -1)
                            {
                                _onParseWrite(message, startIndex, length, background, foreground);
                                startIndex = -1;
                                length = 0;
                            }
                            if (TryGetForegroundColor(escapeCode, isBright, out var color))
                            {
                                foreground = color;
                                isBright = false;
                            }
                            else if (TryGetBackgroundColor(escapeCode, out color))
                            {
                                background = color;
                            }
                            i += 4;
                            continue;
                        }
                    }
                }
                if (startIndex == -1) startIndex = i;
                int nextEscapeIndex = -1;
                if (i < message.Length - 1) nextEscapeIndex = message.IndexOf(EscapeChar, i + 1);
                if (nextEscapeIndex < 0)
                {
                    length = message.Length - startIndex;
                    break;
                }
                length = nextEscapeIndex - startIndex;
                i = nextEscapeIndex - 1;
            }
            if (startIndex != -1) _onParseWrite(message, startIndex, length, background, foreground);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(char c) => (uint)(c - '0') <= ('9' - '0');

        internal const string DefaultForegroundColor = "\x1B[39m\x1B[22m"; // reset to default foreground color
        internal const string DefaultBackgroundColor = "\x1B[49m"; // reset to the background color

        internal static string GetForegroundColorEscapeCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1B[30m",
                ConsoleColor.DarkRed => "\x1B[31m",
                ConsoleColor.DarkGreen => "\x1B[32m",
                ConsoleColor.DarkYellow => "\x1B[33m",
                ConsoleColor.DarkBlue => "\x1B[34m",
                ConsoleColor.DarkMagenta => "\x1B[35m",
                ConsoleColor.DarkCyan => "\x1B[36m",
                ConsoleColor.Gray => "\x1B[37m",
                ConsoleColor.Red => "\x1B[1m\x1B[31m",
                ConsoleColor.Green => "\x1B[1m\x1B[32m",
                ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
                ConsoleColor.Blue => "\x1B[1m\x1B[34m",
                ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
                ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
                ConsoleColor.White => "\x1B[1m\x1B[37m",
                _ => DefaultForegroundColor // default foreground color
            };
        }

        internal static string GetBackgroundColorEscapeCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1B[40m",
                ConsoleColor.DarkRed => "\x1B[41m",
                ConsoleColor.DarkGreen => "\x1B[42m",
                ConsoleColor.DarkYellow => "\x1B[43m",
                ConsoleColor.DarkBlue => "\x1B[44m",
                ConsoleColor.DarkMagenta => "\x1B[45m",
                ConsoleColor.DarkCyan => "\x1B[46m",
                ConsoleColor.Gray => "\x1B[47m",
                _ => DefaultBackgroundColor // Use default background color
            };
        }

        private static bool TryGetForegroundColor(int number, bool isBright, out ConsoleColor? color)
        {
            color = number switch
            {
                30 => ConsoleColor.Black,
                31 => isBright ? ConsoleColor.Red : ConsoleColor.DarkRed,
                32 => isBright ? ConsoleColor.Green : ConsoleColor.DarkGreen,
                33 => isBright ? ConsoleColor.Yellow : ConsoleColor.DarkYellow,
                34 => isBright ? ConsoleColor.Blue : ConsoleColor.DarkBlue,
                35 => isBright ? ConsoleColor.Magenta : ConsoleColor.DarkMagenta,
                36 => isBright ? ConsoleColor.Cyan : ConsoleColor.DarkCyan,
                37 => isBright ? ConsoleColor.White : ConsoleColor.Gray,
                _ => null
            };
            return color != null || number == 39;
        }

        private static bool TryGetBackgroundColor(int number, out ConsoleColor? color)
        {
            color = number switch
            {
                40 => ConsoleColor.Black,
                41 => ConsoleColor.DarkRed,
                42 => ConsoleColor.DarkGreen,
                43 => ConsoleColor.DarkYellow,
                44 => ConsoleColor.DarkBlue,
                45 => ConsoleColor.DarkMagenta,
                46 => ConsoleColor.DarkCyan,
                47 => ConsoleColor.Gray,
                _ => null
            };
            return color != null || number == 49;
        }
    }
}
