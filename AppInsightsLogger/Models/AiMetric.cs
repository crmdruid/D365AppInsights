using System.Runtime.Serialization;

[DataContract]
public class AiMetric
{
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "kind")]
    public string Kind { get; set; }

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

    public AiMetric(string name, DataPointType kind, int value, int? count, int? min, int? max, int? stdDev)
    {
        Name = name.Length > 512
            ? name.Substring(0, 511)
            : name;
        Kind = kind.ToString();
        Value = value;

        if (kind != DataPointType.Aggregation)
            return;

        Count = count ?? 1;
        Min = min ?? value;
        Max = max ?? value;
        StdDev = stdDev ?? 0;
    }
}