namespace Ruminoid.Toolbox.Formatting
{
    public record FormattedEvent
    {
        public FormattedEvent(string target, double progress, string usedTime, string remainingTime)
        {
            Target = target;
            Progress = progress;
            UsedTime = usedTime;
            RemainingTime = remainingTime;
        }

        public string Target { get; }
        public double Progress { get; }
        public string UsedTime { get; }
        public string RemainingTime { get; }
    }
}
