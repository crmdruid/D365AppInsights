# D365AppInsights
## Dynamics 365/CE Application Insights logging for C# plug-ins & workflows

This contains the source code files needed to implement Azure Application Insights logging without needing an external assembly reference.

Supported telemetry types:
* Trace 
* Event 
* Metric
* Dependency 
* Exception

Default D365 Specific Custom Dimensions:
* ExecutionContext
  * InitiatingUserId 
  * ImpersonatingUserId 
  * CorrelationId 
  * Message 
  * Mode 
  * Depth 
  * InputParameters
  * OutputParameters
  * Stage (plug-in)
  * WorkflowCategory (workflow)
  * PrimaryEntityName 
  * PrimaryEntityId 
  * OrganizationName 
* Organization version
* Source (Plug-in or Workflow)

Configurations:
* Application Insights logging endpoint 
* Application Insights instrumentation key
* Disable trace tracking 
* Disable exception tracking 
* Disable dependency tracking 
* Disable metric tracking 
* Disable event tracking 
* Percentage of traces logged 
* Percentage of exceptions logged 
* Percentage of dependencies logged 
* Percentage of metrics logged 
* Percentage of events logged 
* Debug mode

Build Status:  ![Build status](https://jlattimer.visualstudio.com/_apis/public/build/definitions/361a4432-eb0a-46be-bead-c7412245eeae/25/badge)

https://www.nuget.org/packages/JLattimer.D365AppInsights/
