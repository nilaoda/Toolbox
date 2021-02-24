using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Ruminoid.Toolbox.Formatting
{
    [Serializable]
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public record FormattedEvent
    {
        public FormattedEvent()
        {
        }

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

        [DataMember]
        [JsonProperty("target")]
        public string Target;

        /// <summary>
        /// 在0-100之间用来表示进度的浮点数。
        /// </summary>
        [DataMember]
        [JsonProperty("progress")]
        public double Progress;

        [DataMember]
        [JsonProperty("isIndeterminate")]
        public bool IsIndeterminate;

        [DataMember]
        [JsonProperty("summary")]
        public string Summary;

        [DataMember]
        [JsonProperty("detail")]
        public string Detail;

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
