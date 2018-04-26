using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
[KnownType(typeof(AiDependency))]
[KnownType(typeof(AiTrace))]
[KnownType(typeof(AiException))]
[KnownType(typeof(AiEvent))]
public class AiBaseData
{
    [DataMember(Name = "ver")]
    public int Version { get; set; } = 2;

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "properties")]
    public AiProperties Properties { get; set; }

    [DataMember(Name = "exceptions")]
    public List<AiException> Exceptions { get; set; }

    [DataMember(Name = "metrics")]
    public List<AiMetric> Metrics { get; set; }

    [DataMember(Name = "measurements")]
    public AiMeasurements Measurements { get; set; }
}