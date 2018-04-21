var AiFormLogger;
(function (AiFormLogger) {
    enableDebug = false;
    disableTraceTracking = false;
    disableTelemetry = false;
    disableExceptionTracking = false;
    disableAjaxTracking = true;
    disableDependencyTracking = false;
    disableMetricTracking = false;
    disableEventTracking = false;
    targetPage = window;
    function startLogging(config) {
        if (config)
            setConfigOptions(config);
        if (/ClientApiWrapper\.aspx/i.test(window.location.pathname)) {
            targetPage = window.parent;
            if (enableDebug)
                console.log("Application Insights page target: window.parent");
        }
        var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();
        props = {};
        props["entityId"] = Xrm.Page.data.entity.getId().substr(1, 36);
        props["entityName"] = Xrm.Page.data.entity.getEntityName();
        props["formType"] = getFormTypeName(Xrm.Page.ui.getFormType());
        props["orgName"] = Xrm.Page.context.getOrgUniqueName();
        props["orgVersion"] = Xrm.Page.context.getVersion();
        props["source"] = "JavaScript";
        window.appInsights.setAuthenticatedUserContext(Xrm.Page.context.getUserId().substr(1, 36), null, false);
        trackPageView(formName, props);
    }
    AiFormLogger.startLogging = startLogging;
    function setConfigOptions(config) {
        try {
            if (config.hasOwnProperty("enableDebug")) {
                enableDebug = config.enableDebug;
                window.appInsights.config.enableDebug = config.enableDebug;
            }
            if (config.hasOwnProperty("disableTelemetry"))
                window.appInsights.config.disableTelemetry = config.disableTelemetry;
            if (config.hasOwnProperty("verboseLogging"))
                window.appInsights.config.verboseLogging = config.verboseLogging;
            if (config.hasOwnProperty("disableExceptionTracking"))
                window.appInsights.config.disableExceptionTracking = config.disableExceptionTracking;
            if (config.hasOwnProperty("disableAjaxTracking"))
                window.appInsights.config.disableAjaxTracking = config.disableAjaxTracking;
            if (config.hasOwnProperty("maxAjaxCallsPerView"))
                window.appInsights.config.maxAjaxCallsPerView = config.maxAjaxCallsPerView;
        }
        catch (error) {
            console.log("Application Insights error parsing configuration parameters: " + error);
        }
    }
    function trackPageView(formName, props) {
        if (isNaN(targetPage.performance.timing.loadEventEnd) || isNaN(targetPage.performance.timing.responseEnd) ||
            targetPage.performance.timing.loadEventEnd === 0 || targetPage.performance.timing.responseEnd === 0) {
            setTimeout(function () {
                trackPageView(formName, props);
            }, 50);
        }
        else {
            var pageLoad = targetPage.performance.timing.loadEventEnd - targetPage.performance.timing.responseEnd;
            window.appInsights.trackPageView(formName, null, props, null, pageLoad);
            if (enableDebug) {
                console.log("Application Insights page tracking started");
                console.log("Application Insights logged page load time: " + pageLoad + "ms");
            }
        }
    }
    function writeEvent(name, newProps, measurements) {
        if (disableTelemetry || disableEventTracking)
            return;
        window.appInsights.trackEvent(name, combineProps(props, newProps), measurements);
        if (enableDebug)
            console.log("Application Insights logged event: " + name);
    }
    AiFormLogger.writeEvent = writeEvent;
    function writeMetric(name, average, sampleCount, min, max, newProps) {
        if (disableTelemetry || disableMetricTracking)
            return;
        window.appInsights.trackMetric(name, average, sampleCount, min, max, combineProps(props, newProps));
        if (enableDebug)
            console.log("Application Insights logged metric: " + name);
    }
    AiFormLogger.writeMetric = writeMetric;
    function writeException(exception, handledAt, newProps, measurements, severityLevel) {
        if (disableTelemetry || disableExceptionTracking)
            return;
        window.appInsights.trackException(exception, handledAt, combineProps(props, newProps), measurements, severityLevel);
        if (enableDebug)
            console.log("Application Insights logged exception: " + exception.name);
    }
    AiFormLogger.writeException = writeException;
    function writeTrace(message, newProps, severityLevel) {
        if (disableTelemetry || disableTraceTracking)
            return;
        window.appInsights.trackTrace(message, combineProps(props, newProps), severityLevel);
        if (enableDebug)
            console.log("Application Insights logged trace: " + message);
    }
    AiFormLogger.writeTrace = writeTrace;
    function writeDependency(id, method, absoluteUrl, pathName, totalTime, success, resultCode, newProps) {
        if (disableTelemetry || disableDependencyTracking)
            return;
        window.appInsights.trackDependency(id, method, absoluteUrl, pathName, totalTime, success, resultCode, combineProps(props, newProps), null);
        if (enableDebug)
            console.log("Application Insights logged dependency: " + id + ": " + totalTime);
    }
    AiFormLogger.writeDependency = writeDependency;
    function writeMethodTime(methodName, start, end) {
        var time = end - start;
        writeMetric("Method Time: " + methodName, time, null, null, null);
        if (enableDebug)
            console.log("Application Insights logged method time: " + methodName + ": " + time + "ms");
    }
    AiFormLogger.writeMethodTime = writeMethodTime;
    function trackDependencyTime(req, methodName) {
        req.addEventListener("loadstart", function () {
            getStartTime(req, methodName);
        });
        req.addEventListener("load", function () {
            getEndTime(req, true);
        });
        req.addEventListener("error", function () {
            getEndTime(req, false);
        });
    }
    AiFormLogger.trackDependencyTime = trackDependencyTime;
    function getStartTime(req, methodName) {
        req.t0 = performance.now();
        req.methodName = methodName;
    }
    AiFormLogger.getStartTime = getStartTime;
    function getEndTime(req, success) {
        writeDependency(Microsoft.ApplicationInsights.Util.newId(), req._method, req._url, "" + req._url, performance.now() - req.t0, success, req.status, { methodName: req.methodName, mode: getMode(req._async) });
    }
    AiFormLogger.getEndTime = getEndTime;
    function combineProps(props, newProps) {
        if (newProps === null)
            return props;
        for (var attrname in newProps) {
            if (newProps.hasOwnProperty(attrname))
                props[attrname] = newProps[attrname];
        }
        return props;
    }
    function getFormTypeName(formType) {
        switch (formType) {
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
    function getMode(mode) {
        return (mode) ? "Asynchronous" : "Synchronous";
    }
})(AiFormLogger || (AiFormLogger = {}));
var xhrProto = XMLHttpRequest.prototype, origOpen = xhrProto.open;
xhrProto.open = function (method, url, async) {
    this._url = url;
    this._method = method;
    this._async = async;
    return origOpen.apply(this, arguments);
};
