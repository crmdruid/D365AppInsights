using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiData
    {
        [DataMember(Name = "baseType")]
        public string BaseType { get; set; }

        [DataMember(Name = "baseData")]
        public AiBaseData BaseData { get; set; }
    }
}