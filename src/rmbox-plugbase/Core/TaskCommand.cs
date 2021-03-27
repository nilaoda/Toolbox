namespace Ruminoid.Toolbox.Core
{
    public record TaskCommand
    {
        public string Target = "";
        public string Args = "";
        public string Formatter = "";

        public static implicit operator (string Target, string Args, string Formatter)(TaskCommand command) =>
            (command.Target, command.Args, command.Formatter);

        public static implicit operator TaskCommand((string Target, string Args, string Formatter) command) =>
            new() {Target = command.Target, Args = command.Args, Formatter = command.Formatter};
    }
}
