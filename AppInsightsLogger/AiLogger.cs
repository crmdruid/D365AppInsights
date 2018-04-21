using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

public class AiLogger
{
    private readonly string _instrumentationKey;
    private readonly string _loggingEndpoint;
    private readonly string _applicationVersion;
    private readonly bool _disableTraceTracking;
    private readonly bool _disableExceptionTracking;
    private readonly bool _disableDependencyTracking;
    private readonly bool _disableMetricTracking;
    private readonly bool _disableEventTracking;
    private readonly string _authenticatedUserId;
    private readonly ITracingService _tracingService;
    private readonly AiProperties _eventProperties;
    private static HttpClient _httpClient;

    public AiLogger(string secureConfig, IOrganizationService service, ITracingService tracingService,
        IExecutionContext executionContext, Dictionary<string, object> workflowContextDetails = null)
    {
        AiConfig aiConfig = new AiConfig(secureConfig);
        _instrumentationKey = aiConfig.InstrumentationKey;
        _loggingEndpoint = aiConfig.AiEndpoint;
        _applicationVersion = GetVersion(service);
        _disableTraceTracking = aiConfig.DisableTraceTracking;
        _disableExceptionTracking = aiConfig.DisableExceptionTracking;
        _disableDependencyTracking = aiConfig.DisableDependencyTracking;
        _disableEventTracking = aiConfig.DisableEventTracking;
        _disableMetricTracking = aiConfig.DisableMetricTracking;
        bool disableContextParameterTracking = aiConfig.DisableContextParameterTracking;
        _authenticatedUserId = executionContext.InitiatingUserId.ToString();
        _tracingService = tracingService;
        _httpClient = HttpHelper.GetHttpClient();

        _eventProperties = new AiProperties();
        if (!disableContextParameterTracking)
            AddExecutionContextDetails(executionContext, service, workflowContextDetails);
    }

    public bool WriteTrace(string message, AiTraceSeverity aiTraceSeverity)
    {
        if (_disableTraceTracking)
            return true;

        AiTrace aiTrace = new AiTrace(message, aiTraceSeverity);

        string json = GetTraceJsonString(aiTrace);
        return SendToAi(json);
    }

    public bool WriteEvent(string name, Dictionary<string, double?> measurements)
    {
        if (_disableEventTracking)
            return true;

        AiEvent aiEvent = new AiEvent(name);

        string json = GetEventJsonString(aiEvent, measurements);
        return SendToAi(json);
    }

    public bool WriteException(Exception e, AiExceptionSeverity severity)
    {
        if (_disableExceptionTracking)
            return true;

        AiException aiException = new AiException(e, severity);

        string json = GetExceptionJsonString(aiException);
        return SendToAi(json);
    }

    public bool WriteMetric(string name, DataPointType kind, int value, int? count, int? min, int? max, int? stdDev)
    {
        if (_disableMetricTracking)
            return true;

        AiMetric metric = new AiMetric(name, kind, value, count, min, max, stdDev);

        string json = GetMetricJsonString(metric);
        return SendToAi(json);
    }

    public bool WriteDependency(string name, string method, string type, int duration, int? resultCode,
        bool success, string data)
    {
        if (_disableDependencyTracking)
            return true;

        AiDependency dependency =
            new AiDependency(_eventProperties, name, method, type, duration, resultCode, success, data);

        string json = GetDependencyJsonString(dependency, null);
        return SendToAi(json);
    }

    private bool SendToAi(string json)
    {
        try
        {
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/x-json-stream");
            HttpResponseMessage response = _httpClient.PostAsync(_loggingEndpoint, content).Result;

            if (response.IsSuccessStatusCode)
                return true;

            _tracingService?.Trace(
                $"Error writing to Application Insights with response: {response.StatusCode.ToString()}: {response.ReasonPhrase}: Message: {json}");
            return false;
        }
        catch (Exception e)
        {
            _tracingService?.Trace($"Error writing to Application Insights: Message: {json}", e);
            return false;
        }
    }

    private string GetTraceJsonString(AiTrace aiTrace)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Message",
            Time = $"{DateTime.UtcNow:O}",
            InstrumentationKey = _instrumentationKey,
            Tags = new AiTags
            {
                OperationName = null,
                RoleInstance = null,
                AuthenticatedUserId = _authenticatedUserId
            },
            Data = new AiData
            {
                BaseType = "MessageData",
                BaseData = aiTrace
            }
        };

        var json = SerializationHelper.SerializeObject<AiLogRequest>(logRequest);
        return json;
    }

    private string GetEventJsonString(AiEvent aiEvent, Dictionary<string, double?> measurements)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Event",
            Time = $"{DateTime.UtcNow:O}",
            InstrumentationKey = _instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = null,
                OperationName = null,
                AuthenticatedUserId = _authenticatedUserId
            },
            Data = new AiData
            {
                BaseType = "EventData",
                BaseData = aiEvent
            }
        };

        var json = SerializationHelper.SerializeObject<AiLogRequest>(logRequest);
        json = InsertMetricsJson(measurements, json);

        return json;
    }

    private static string InsertMetricsJson(Dictionary<string, double?> measurements, string json)
    {
        if (measurements == null)
            return json;

        string replacement = "\"measurements\":{";

        int i = 0;
        foreach (KeyValuePair<string, double?> keyValuePair in measurements)
        {
            i++;
            replacement += $"\"{keyValuePair.Key}\": {keyValuePair.Value}";
            if (i != measurements.Count)
                replacement += ", ";
        }

        replacement += "}";

        json = json.Replace("\"measurements\":null", replacement);

        return json;
    }

    private string GetExceptionJsonString(AiException aiException)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Exception",
            Time = $"{DateTime.UtcNow:O}",
            InstrumentationKey = _instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = null,
                OperationName = null,
                AuthenticatedUserId = _authenticatedUserId
            },
            Data = new AiData
            {
                BaseType = "ExceptionData",
                BaseData = new AiBaseData
                {
                    Properties = _eventProperties,
                    Exceptions = new List<AiException> {
                        aiException
                    }
                }
            }
        };

        var json = SerializationHelper.SerializeObject<AiLogRequest>(logRequest);
        return json;
    }

    private string GetMetricJsonString(AiMetric aiMetric)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Metric",
            Time = $"{DateTime.UtcNow:O}",
            InstrumentationKey = _instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = null,
                OperationName = null,
                ApplicationVersion = _applicationVersion,
                AuthenticatedUserId = _authenticatedUserId
            },
            Data = new AiData
            {
                BaseType = "MetricData",
                BaseData = new AiBaseData
                {
                    Metrics = new List<AiMetric> {
                        aiMetric
                    },
                    Properties = _eventProperties
                }
            }
        };

        var json = SerializationHelper.SerializeObject<AiLogRequest>(logRequest);
        return json;
    }

    private string GetDependencyJsonString(AiBaseData aiDependency, string operationName)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.RemoteDependency",
            Time = $"{DateTime.UtcNow:O}",
            InstrumentationKey = _instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = "",
                OperationName = operationName,
                AuthenticatedUserId = _authenticatedUserId
            },
            Data = new AiData
            {
                BaseType = "RemoteDependencyData",
                BaseData = aiDependency
            }
        };

        var json = SerializationHelper.SerializeObject<AiLogRequest>(logRequest);
        return json;
    }

    private void AddExecutionContextDetails(IExecutionContext executionContext, IOrganizationService service,
        Dictionary<string, object> workflowContextDetails)
    {
        _eventProperties.ImpersonatingUserId = executionContext.UserId.ToString();
        _eventProperties.CorrelationId = executionContext.CorrelationId.ToString();
        _eventProperties.EntityName = executionContext.PrimaryEntityName;
        _eventProperties.EntityId = executionContext.PrimaryEntityId.ToString();
        _eventProperties.Message = executionContext.MessageName;
        _eventProperties.Mode = AiProperties.GetModeName(executionContext.Mode);
        _eventProperties.Depth = executionContext.Depth;
        _eventProperties.OrgName = executionContext.OrganizationName;
        _eventProperties.OrgVersion = GetVersion(service);
        _eventProperties.InputParameters = TraceParameters(true, executionContext);
        _eventProperties.OutputParameters = TraceParameters(false, executionContext);

        if (executionContext.GetType().Name.Contains("PluginExecutionContext"))
            AddPluginExecutionContextDetails((IPluginExecutionContext)executionContext);
        _tracingService.Trace(executionContext.GetType().Name);
        if (executionContext.GetType().Name.Contains("CodeActivityContext")
            || executionContext.GetType().Name.Contains("WorkflowContext") && workflowContextDetails != null)
            AddWorkflowExecutionContextDetails(workflowContextDetails);
    }

    private void AddPluginExecutionContextDetails(IPluginExecutionContext pluginExecutionContext)
    {
        _eventProperties.Stage = AiProperties.GetStageName(pluginExecutionContext.Stage);
        _eventProperties.Source = "Plug-in";
    }

    private void AddWorkflowExecutionContextDetails(Dictionary<string, object> workflowContextDetails)
    {
        _eventProperties.Source = "Workflow";
        bool hasWorkflowCategory = workflowContextDetails.TryGetValue("WorkflowCategory", out object workflowCategory);
        if (hasWorkflowCategory)
            _eventProperties.WorkflowCategory = workflowCategory.ToString();
    }

    private static string TrimPropertyValueLength(string value)
    {
        return value.Length > 8192
            ? value.Substring(0, 8191)
            : value;
    }

    private static string GetVersion(IOrganizationService service)
    {
        RetrieveVersionRequest request = new RetrieveVersionRequest();
        RetrieveVersionResponse response = (RetrieveVersionResponse)service.Execute(request);

        return response.Version;
    }

    private string TraceParameters(bool input, IExecutionContext executionContext)
    {
        ParameterCollection parameters = input
            ? executionContext.InputParameters
            : executionContext.OutputParameters;

        if (parameters == null || parameters.Count == 0)
            return null;

        try
        {
            StringBuilder sb = new StringBuilder();

            int i = 1;
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                var typeFullname = (object)parameter.Value.GetType().FullName;
                string parameterType = input ? "Input" : "Output";
                sb.Append($"{parameterType} Parameter({typeFullname}): {parameter.Key}: ");

                switch (typeFullname)
                {
                    case "System.String":
                    case "System.Decimal":
                    case "System.Int32":
                    case "System.Boolean":
                    case "System.Single":
                        sb.Append(parameter.Value);
                        break;
                    case "System.DateTime":
                        sb.Append(((DateTime)parameter.Value).ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz"));
                        break;
                    case "Microsoft.Xrm.Sdk.EntityReference":
                        var entityReference = (EntityReference)parameter.Value;
                        sb.Append($"Id: {entityReference.Id}, LogicalName: {entityReference.LogicalName}");
                        break;
                    case "Microsoft.Xrm.Sdk.Entity":
                        var entity = (Entity)parameter.Value;
                        sb.Append($"Id: {entity.Id}, LogicalName: {entity.LogicalName}");
                        break;
                    case "Microsoft.Xrm.Sdk.EntityCollection":
                        var entityCollection = (EntityCollection)parameter.Value;
                        foreach (var e in entityCollection.Entities)
                        {
                            sb.Append(Environment.NewLine);
                            sb.Append($"Id: {e.Id}, LogicalName: {e.LogicalName}");
                        }
                        break;
                    case "Microsoft.Xrm.Sdk.OptionSetValue":
                        sb.Append(((OptionSetValue)parameter.Value).Value);
                        break;
                    case "Microsoft.Xrm.Sdk.Money":
                        sb.Append(((Money)parameter.Value).Value.ToString(CultureInfo.CurrentCulture));
                        break;
                    default:
                        sb.Append($"Undefined Type - {typeFullname}");
                        break;
                }

                i++;
                if (i < parameters.Count)
                    sb.Append(Environment.NewLine);
            }

            string result = sb.ToString();
            return result.Length > 8192
                ? result.Substring(0, 8191)
                : result;
        }
        catch (Exception e)
        {
            _tracingService.Trace($"Error tracing parameters: {e.Message}");
            return null;
        }
    }
}