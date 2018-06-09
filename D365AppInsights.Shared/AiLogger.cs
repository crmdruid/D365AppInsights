using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace JLattimer.D365AppInsights
{
    public class AiLogger
    {
        private string _instrumentationKey;
        private string _loggingEndpoint;
        private bool _disableTraceTracking;
        private bool _disableExceptionTracking;
        private bool _disableDependencyTracking;
        private bool _disableMetricTracking;
        private bool _disableEventTracking;
        private bool _enableDebug;
        private string _authenticatedUserId;
        private ITracingService _tracingService;
        private AiProperties _eventProperties;
        private static HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AiLogger"/> class.
        /// </summary>
        /// <param name="aiSetupJson">AiSetup json.</param>
        /// <param name="service">D365 IOrganizationService.</param>
        /// <param name="tracingService">D365 ITracingService.</param>
        /// <param name="executionContext">D365 IExecutionContext (IPluginExecutionContext or IWorkflowContext).</param>
        /// <param name="pluginStage">Plug-in stage from context</param>
        /// <param name="workflowCategory">Workflow category from context</param>
        public AiLogger(string aiSetupJson, IOrganizationService service, ITracingService tracingService,
            IExecutionContext executionContext, int? pluginStage, int? workflowCategory)
        {
            ValidateContextSpecific(pluginStage, workflowCategory);
            AiConfig aiConfig = new AiConfig(aiSetupJson);
            SetupAiLogger(aiConfig, service, tracingService, executionContext, pluginStage, workflowCategory);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AiLogger"/> class.
        /// </summary>
        /// <param name="aiConfig">AiConfiguration.</param>
        /// <param name="service">D365 IOrganizationService.</param>
        /// <param name="tracingService">D365 ITracingService.</param>
        /// <param name="executionContext">D365 IExecutionContext (IPluginExecutionContext or IWorkflowContext).</param>
        /// <param name="pluginStage">Plug-in stage from context</param>
        /// <param name="workflowCategory">Workflow category from context</param>
        public AiLogger(AiConfig aiConfig, IOrganizationService service, ITracingService tracingService,
            IExecutionContext executionContext, int? pluginStage, int? workflowCategory)
        {
            ValidateContextSpecific(pluginStage, workflowCategory);
            SetupAiLogger(aiConfig, service, tracingService, executionContext, pluginStage, workflowCategory);
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void ValidateContextSpecific(int? pluginStage, int? workflowCategory)
        {
            if (pluginStage == null && workflowCategory == null)
                throw new InvalidPluginExecutionException(
                    "Either Plug-in Stage or Workflow Category must be passed to AiLogger");
        }

        private void SetupAiLogger(AiConfig aiConfig, IOrganizationService service, ITracingService tracingService,
            IExecutionContext executionContext, int? pluginStage, int? workflowCategory)
        {
            _instrumentationKey = aiConfig.InstrumentationKey;
            _loggingEndpoint = aiConfig.AiEndpoint;
            _disableTraceTracking = aiConfig.DisableTraceTracking;
            _disableExceptionTracking = aiConfig.DisableExceptionTracking;
            _disableDependencyTracking = aiConfig.DisableDependencyTracking;
            _disableEventTracking = aiConfig.DisableEventTracking;
            _disableMetricTracking = aiConfig.DisableMetricTracking;
            _enableDebug = aiConfig.EnableDebug;
            bool disableContextParameterTracking = aiConfig.DisableContextParameterTracking;
            _authenticatedUserId = executionContext.InitiatingUserId.ToString();
            _tracingService = tracingService;
            _httpClient = HttpHelper.GetHttpClient();

            _eventProperties = new AiProperties();
            AddPrimaryPropertyValues(executionContext, service);

            if (!disableContextParameterTracking)
                AddExecutionContextDetails(executionContext, pluginStage, workflowCategory);
        }

        /// <summary>
        /// Writes a trace message to Application Insights.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="aiTraceSeverity">The severity level <see cref="AiTraceSeverity"/>.</param>
        /// <param name="timestamp">The UTC timestamp of the event (default = DateTime.UtcNow).</param>
        /// <returns><c>true</c> if successfully logged, <c>false</c> otherwise.</returns>
        public bool WriteTrace(string message, AiTraceSeverity aiTraceSeverity, DateTime? timestamp = null)
        {
            if (_disableTraceTracking)
                return true;

            timestamp = timestamp ?? DateTime.UtcNow;

            AiTrace aiTrace = new AiTrace(_eventProperties, message, aiTraceSeverity);

            string json = GetTraceJsonString(timestamp.Value, aiTrace);

            if (_enableDebug)
                _tracingService.Trace($"DEBUG: Application Insights JSON: {CreateJsonDataLog(json)}");

            return SendToAi(json);
        }

        /// <summary>
        /// Writes an event message to Application Insights.
        /// </summary>
        /// <param name="name">The event name.</param>
        /// <param name="measurements">The associated measurements.</param>
        /// <param name="timestamp">The UTC timestamp of the event (default = DateTime.UtcNow).</param>
        /// <returns><c>true</c> if successfully logged, <c>false</c> otherwise.</returns>
        public bool WriteEvent(string name, Dictionary<string, double?> measurements, DateTime? timestamp = null)
        {
            if (_disableEventTracking)
                return true;

            timestamp = timestamp ?? DateTime.UtcNow;

            AiEvent aiEvent = new AiEvent(_eventProperties, name);

            string json = GetEventJsonString(timestamp.Value, aiEvent, measurements);

            if (_enableDebug)
                _tracingService.Trace($"DEBUG: Application Insights JSON: {CreateJsonDataLog(json)}");

            return SendToAi(json);
        }

        /// <summary>
        /// Writes exception data to Application Insights.
        /// </summary>
        /// <param name="exception">The exception being logged.</param>
        /// <param name="aiExceptionSeverity">The severity level <see cref="AiExceptionSeverity"/>.</param>
        /// <param name="timestamp">The UTC timestamp of the event (default = DateTime.UtcNow).</param>
        /// <returns><c>true</c> if successfully logged, <c>false</c> otherwise.</returns>
        public bool WriteException(Exception exception, AiExceptionSeverity aiExceptionSeverity, DateTime? timestamp = null)
        {
            if (_disableExceptionTracking)
                return true;

            timestamp = timestamp ?? DateTime.UtcNow;

            AiException aiException = new AiException(exception, aiExceptionSeverity);

            string json = GetExceptionJsonString(timestamp.Value, aiException);

            if (_enableDebug)
                _tracingService.Trace($"DEBUG: Application Insights JSON: {CreateJsonDataLog(json)}");

            return SendToAi(json);
        }

        /// <summary>
        /// Writes a metric message to Application Insights.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="count">The count of metrics being logged (default = 1).</param>
        /// <param name="min">The minimum value of metrics being logged (default = value).</param>
        /// <param name="max">The maximum value of metrics being logged (default = value).</param>
        /// <param name="stdDev">The standard deviantion of metrics being logged (default = 0).</param>
        /// <param name="timestamp">The UTC timestamp of the event (default = DateTime.UtcNow).</param>
        /// <returns><c>true</c> if successfully logged, <c>false</c> otherwise.</returns>
        public bool WriteMetric(string name, int value, int? count, int? min, int? max, int? stdDev, DateTime? timestamp = null)
        {
            if (_disableMetricTracking)
                return true;

            timestamp = timestamp ?? DateTime.UtcNow;

            AiMetric metric = new AiMetric(name, value, count, min, max, stdDev);

            string json = GetMetricJsonString(timestamp.Value, metric);

            if (_enableDebug)
                _tracingService.Trace($"DEBUG: Application Insights JSON: {CreateJsonDataLog(json)}");

            return SendToAi(json);
        }

        /// <summary>
        /// Writes a dependency message to Application Insights.
        /// </summary>
        /// <param name="name">The dependency name or absolute URL.</param>
        /// <param name="method">The HTTP method (only logged with URL).</param>
        /// <param name="type">The type of dependency (Ajax, HTTP, SQL, etc.).</param>
        /// <param name="duration">The duration in ms of the dependent event.</param>
        /// <param name="resultCode">The result code, HTTP or otherwise.</param>
        /// <param name="success">Set to <c>true</c> if the dependent event was successful, <c>false</c> otherwise.</param>
        /// <param name="data">Any other data associated with the dependent event.</param>
        /// <param name="timestamp">The UTC timestamp of the dependent event (default = DateTime.UtcNow).</param>
        /// <returns><c>true</c> if successfully logged, <c>false</c> otherwise.</returns>
        public bool WriteDependency(string name, string method, string type, int duration, int? resultCode,
            bool success, string data, DateTime? timestamp = null)
        {
            if (_disableDependencyTracking)
                return true;

            timestamp = timestamp ?? DateTime.UtcNow;

            AiDependency dependency =
                new AiDependency(_eventProperties, name, method, type, duration, resultCode, success, data);

            string json = GetDependencyJsonString(timestamp.Value, dependency, null);

            if (_enableDebug)
                _tracingService.Trace($"DEBUG: Application Insights JSON: {CreateJsonDataLog(json)}");

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
                    $"Error writing to Application Insights with response: {response.StatusCode.ToString()}: {response.ReasonPhrase}: Message: {CreateJsonDataLog(json)}");
                return false;
            }
            catch (Exception e)
            {
                _tracingService?.Trace(CreateJsonDataLog(json), e);
                return false;
            }
        }

        private string GetTraceJsonString(DateTime timestamp, AiTrace aiTrace)
        {
            AiLogRequest logRequest = new AiLogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Message",
                Time = timestamp.ToString("O"),
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

        private string GetEventJsonString(DateTime timestamp, AiEvent aiEvent, Dictionary<string, double?> measurements)
        {
            AiLogRequest logRequest = new AiLogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Event",
                Time = timestamp.ToString("O"),
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

        private string GetExceptionJsonString(DateTime timestamp, AiException aiException)
        {
            AiLogRequest logRequest = new AiLogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Exception",
                Time = timestamp.ToString("O"),
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

        private string GetMetricJsonString(DateTime timestamp, AiMetric aiMetric)
        {
            AiLogRequest logRequest = new AiLogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.Metric",
                Time = timestamp.ToString("O"),
                InstrumentationKey = _instrumentationKey,
                Tags = new AiTags
                {
                    RoleInstance = null,
                    OperationName = null,
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

        private string GetDependencyJsonString(DateTime timestamp, AiBaseData aiDependency, string operationName)
        {
            AiLogRequest logRequest = new AiLogRequest
            {
                Name = $"Microsoft.ApplicationInsights.{_instrumentationKey}.RemoteDependency",
                Time = timestamp.ToString("O"),
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

        private void AddPrimaryPropertyValues(IExecutionContext executionContext, IOrganizationService service)
        {
            _eventProperties.EntityName = executionContext.PrimaryEntityName;
            _eventProperties.EntityId = executionContext.PrimaryEntityId.ToString();
            _eventProperties.OrgName = executionContext.OrganizationName;
            _eventProperties.OrgVersion = GetVersion(service);
        }

        private void AddExecutionContextDetails(IExecutionContext executionContext, int? pluginStage, int? workflowCategory)
        {
            _eventProperties.ImpersonatingUserId = executionContext.UserId.ToString();
            _eventProperties.CorrelationId = executionContext.CorrelationId.ToString();
            _eventProperties.Message = executionContext.MessageName;
            _eventProperties.Mode = AiProperties.GetModeName(executionContext.Mode);
            _eventProperties.Depth = executionContext.Depth;
            _eventProperties.InputParameters = TraceParameters(true, executionContext);
            _eventProperties.OutputParameters = TraceParameters(false, executionContext);

            if (pluginStage != null)
            {
                int stage = pluginStage.Value;
                AddPluginExecutionContextDetails(stage);
            }

            if (workflowCategory != null)
            {
                int category = workflowCategory.Value;
                AddWorkflowExecutionContextDetails(category);
            }
        }

        private void AddPluginExecutionContextDetails(int stage)
        {
            _eventProperties.Source = "Plug-in";
            _eventProperties.Stage = AiProperties.GetStageName(stage);
        }

        private void AddWorkflowExecutionContextDetails(int workflowCategory)
        {
            _eventProperties.Source = "Workflow";
            _eventProperties.WorkflowCategory = GetWorkflowCategoryName(workflowCategory);
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
                    var typeFullname = (object)parameter.Value?.GetType().FullName;
                    string parameterType = input ? "Input" : "Output";
                    sb.Append($"{parameterType} Parameter({typeFullname}): {parameter.Key}: ");

                    switch (typeFullname)
                    {
                        case "System.String":
                        case "System.Decimal":
                        case "System.Double":
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
                    if (i <= parameters.Count)
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

        private static string CreateJsonDataLog(string json)
        {
            return json.Replace("{", "{{").Replace("}", "}}");
        }

        private static string GetWorkflowCategoryName(int category)
        {
            switch (category)
            {
                case 0:
                    return "Workflow";
                case 1:
                    return "Dialog";
                case 2:
                    return "Business Rule";
                case 3:
                    return "Action";
                case 4:
                    return "Business Process Flow";
                default:
                    return "Unknown";
            }
        }
    }
}