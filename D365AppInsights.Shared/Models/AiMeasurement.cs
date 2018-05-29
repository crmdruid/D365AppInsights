using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiMeasurements
    {
        public Dictionary<string, double> Measurements { get; set; }
    }
}