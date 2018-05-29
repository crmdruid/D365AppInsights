using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiEvent : AiBaseData
    {
        public AiEvent(string name)
        {
            Name = name.Length > 512
                ? name.Substring(0, 511)
                : name;
        }

        public static string ValidateMeasurementName(string mesaurementName)
        {
            if (mesaurementName.Length > 150)
                return "Measurement name cannot exceed 150 characters";

            return null;
        }
    }
}