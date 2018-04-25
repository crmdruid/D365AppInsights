/*
  Add this function to form's OnLoad handler
  AiFormLogger.startLogging
 
  Pass this JSON string to the function and set values as needed

  {
	"enableDebug": true,
	"disablePageviewTracking": false,
	"disablePageLoadTimeTracking": false,
	"disableExceptionTracking": false,
	"disableAjaxTracking": true,
	"maxAjaxCallsPerView": 500,
	"disableTraceTracking": false,
	"disableDependencyTracking": false,
	"disableMetricTracking": false,
	"disableEventTracking": false
  }

*/

/*
 https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md
 {
        "enableDebug": false,  //Turns on/off built in AI debug mode
        "disablePageviewTracking": false, //Turns on/off Pageview tracking
        "disablePageLoadTimeTracking": false, //Turns on/off metric recording page load duration
        "disableExceptionTracking": false, //Turns on/off built in AI exception tracking and custom implementation
        "disableAjaxTracking": true, //Turns on/off built in AI request tracking which logs all Ajax requests
        "maxAjaxCallsPerView": 500, //The max number of requests logged using the built in tracking
        "disableTraceTracking": false, //Turns on/off custom implementation of trace tracking
        "disableDependencyTracking": false, //Turns on/off custom implementation of manual dependency tracking
        "disableMetricTracking": false, //Turns on/off custom implementation of metric tracking
        "disableEventTracking": false //Turns on/off custom implementation of event tracking
   }
*/