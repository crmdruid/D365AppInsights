function Trace() {

    AiFormLogger.writeTrace("Test trace", null, AI.SeverityLevel.Warning);

    var pageLoad = window.parent.performance.timing.loadEventEnd - window.parent.performance.timing.navigationStart;
    console.log("loadEventEnd: " + window.parent.performance.timing.loadEventEnd);
    console.log("navigationStart: " + window.parent.performance.timing.navigationStart);
    console.log(`Application Insights logged page load time (navigationStart): ${pageLoad}ms`);
    console.log("? " + window.parent.performance.getEntriesByType("navigation")[0].loadEventEnd);
}