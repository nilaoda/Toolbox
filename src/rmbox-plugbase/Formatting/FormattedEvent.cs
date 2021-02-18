using System.Text;

namespace Ruminoid.Toolbox.Formatting
{
    public record FormattedEvent
    {
        public FormattedEvent(
            string target,
            double progress,
            bool isIndeterminate,
            string summary,
            string detail) =>
            (
                Target,
                Progress,
                IsIndeterminate,
                Summary,
                Detail
            ) =
            (
                target,
                progress,
                isIndeterminate,
                summary,
                detail
            );

        public string Target { get; }
        public double Progress { get; }
        public bool IsIndeterminate { get; }
        public string Summary { get; }
        public string Detail { get; }

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            builder.Append('[');
            builder.Append(Target);
            builder.Append(']');
            if (!IsIndeterminate) builder.Append(Progress);

            if (!IsIndeterminate &&
                !string.IsNullOrWhiteSpace(Summary))

                builder.Append('|');

            builder.Append(Summary);
            if (!string.IsNullOrWhiteSpace(Detail))
            {
                builder.Append('|');
                builder.Append(Detail);
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            PrintMembers(builder);
            return builder.ToString();
        }
    }
}