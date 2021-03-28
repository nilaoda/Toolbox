namespace Ruminoid.Toolbox.Core
{
    public record TaskCommand
    {
        public TaskCommand()
        {
        }

        public TaskCommand(string target, string args, string formatter)
        {
            Target = target;
            Args = args;
            Formatter = formatter;
        }

        public readonly string Target = "";
        public readonly string Args = "";
        public readonly string Formatter = "";

        public static implicit operator (string Target, string Args, string Formatter)(TaskCommand command) =>
            (command.Target, command.Args, command.Formatter);

        public static implicit operator TaskCommand((string Target, string Args, string Formatter) command) =>
            new(command.Target, command.Args, command.Formatter);
    }
}
