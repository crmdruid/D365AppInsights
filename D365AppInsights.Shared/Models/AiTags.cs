using System.Runtime.Serialization;

[DataContract]
public class AiTags
{
    [DataMember(Name = "ai.operation.name")]
    public string OperationName { get; set; }

    [DataMember(Name = "ai.cloud.roleInstance")]
    public string RoleInstance { get; set; }

    [DataMember(Name = "ai.operation.id")]
    public string OperationId { get; set; }

    [DataMember(Name = "ai.user.authUserId")]
    public string AuthenticatedUserId { get; set; }

    [DataMember(Name = "ai.operation.parentid")]
    public string OperationParentId { get; set; }

    [DataMember(Name = "application_Version")]
    public string ApplicationVersion { get; set; }
}