D365AppInsights


Getting started:
1. Create a JSON string matching what is found in the AiSetup class or and instance of the AiConfig class to determine loggin configuration
   At a minimum your Application Insights instrumentation key needs to be set
2. Create a new instance of the AiLogger class and provide the AiSetup JSON or AiConfig, IOrganizationService, ITracingService, and optionally the plug-in stage or workflow category


For examples: 
https://github.com/jlattimer/D365AppInsights/wiki/Examples


Additional information can be found on the GitHub site:
https://github.com/jlattimer/D365AppInsights