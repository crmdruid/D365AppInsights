using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiMetric
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public int Value { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "min")]
        public int Min { get; set; }

        [DataMember(Name = "max")]
        public int Max { get; set; }

        [DataMember(Name = "stdDev")]
        public int StdDev { get; set; }

        [DataMember(Name = "kind")]
        public int Kind { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AiMetric"/> class.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="count">The count of metrics being logged (default = 1).</param>
        /// <param name="min">The minimum value of metrics being logged (default = value).</param>
        /// <param name="max">The maximum value of metrics being logged (default = value).</param>
        /// <param name="stdDev">The standard deviantion of metrics being logged (default = 0).</param>
        public AiMetric(string name, int value, int? count, int? min, int? max, int? stdDev)
        {
            Name = name.Length > 512
                ? name.Substring(0, 511)
                : name;
            Value = value;

            Count = count ?? 1;
            Min = min ?? value;
            Max = max ?? value;
            StdDev = stdDev ?? 0;
            Kind = count == 1
                ? (int)AiDataPointType.Measurement
                : (int)AiDataPointType.Aggregation;
        }
    }
}