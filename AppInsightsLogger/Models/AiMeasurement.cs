using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class AiMeasurements
{
    public Dictionary<string, double> Measurements { get; set; }
}