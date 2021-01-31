namespace Ruminoid.Toolbox.Formatting
{
    public record FormattedEvent
    {
        public string Target { get; }
        public string UsedTime { get; }
        public string RemainingTime { get; }
        public double Progress { get; }
    }
}
