using System.Runtime.Serialization;

namespace JLattimer.D365AppInsights
{
    [DataContract]
    public class AiProperties
    {
        //Define additional properties as needed
        [DataMember(Name = "impersonatingUserId")]
        public string ImpersonatingUserId { get; set; }

        [DataMember(Name = "entityId")]
        public string EntityId { get; set; }

        [DataMember(Name = "entityName")]
        public string EntityName { get; set; }

        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "stage")]
        public string Stage { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "depth")]
        public int Depth { get; set; }

        [DataMember(Name = "workflowCategory")]
        public string WorkflowCategory { get; set; }

        [DataMember(Name = "formId")]
        public string FormId { get; set; }

        [DataMember(Name = "formType")]
        public string FormType { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "methodName")]
        public string MethodName { get; set; }

        [DataMember(Name = "orgName")]
        public string OrgName { get; set; }

        [DataMember(Name = "orgVersion")]
        public string OrgVersion { get; set; }

        [DataMember(Name = "inputParameters")]
        public string InputParameters { get; set; }

        [DataMember(Name = "outputParameters")]
        public string OutputParameters { get; set; }

        public static string GetStageName(int stage)
        {
            switch (stage)
            {
                case 10:
                    return "Pre-validation";
                case 20:
                    return "Pre-operation";
                case 40:
                    return "Post-operation";
                default:
                    return "MainOperation";
            }
        }

        public static string GetModeName(int mode)
        {
            return mode == 0 ? "Synchronous" : "Asynchronous";
        }

        public static string GetFormTypeName(int formType)
        {
            switch (formType)
            {
                case 1:
                    return "Create";
                case 2:
                    return "Update";
                case 3:
                    return "Read Only";
                case 4:
                    return "Disabled";
                case 6:
                    return "Bulk Edit";
                default:
                    return "Undefined";
            }
        }
    }
}