using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiEvent : AiBaseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AiEvent"/> class.
        /// </summary>
        /// <param name="properties">The D365 specific AI properties.</param>
        /// <param name="name">The event name.</param>
        public AiEvent(AiProperties properties, string name)
        {
            Name = name.Length > 512
                ? name.Substring(0, 511)
                : name;
            Properties = properties;
        }

        public static string ValidateMeasurementName(string mesaurementName)
        {
            if (mesaurementName.Length > 150)
                return "Invalid Measurement: name cannot exceed 150 characters";

            return null;
        }
    }
}