/// <reference path="../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../scripts/ai.1.0.15-build03916.d.ts" />

namespace AiFormLogger {
    declare var impersonatingUserId: string;
    declare var enableDebug: boolean;
    declare var props: Object;
    declare var disableTelemetry: boolean;
    declare var disableTraceTracking: boolean;
    declare var disableExceptionTracking: boolean;
    declare var disableAjaxTracking: boolean;
    declare var disableDependencyTracking: boolean;
    declare var disableMetricTracking: boolean;
    declare var disableEventTracking: boolean;
    declare var targetPage: any;
    enableDebug = false;
    disableTraceTracking = false;
    disableTelemetry = false;
    disableExceptionTracking = false;
    disableAjaxTracking = true;
    disableDependencyTracking = false;
    disableMetricTracking = false;
    disableEventTracking = false;
    targetPage = window;

    export function startLogging(config?: any) {

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

        (window as any).appInsights.setAuthenticatedUserContext(Xrm.Page.context.getUserId().substr(1, 36), null, false);

        trackPageView(formName, props);
    }

    function setConfigOptions(config: any) {
        try {
            //https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md
            /* {
                    "enableDebug": false,  //Turns on/off built in AI debug mode
                    "disableTelemetry": false, //Turns on/off all telemetry data
                    "verboseLogging": false,  //Turns on/off built in AI verbose logging
                    "disableExceptionTracking": false, //Turns on/off built in AI exception tracking and custom implementation
                    "disableAjaxTracking": true, //Turns on/off built in AI request tracking which logs all requests
                    "maxAjaxCallsPerView": 500, //The max number of requests logged using the built in tracking
                    "disableTraceTracking": false, //Turns on/off custom implementation of trace tracking
                    "disableDependencyTracking": false, //Turns on/off custom implementation of manual dependency tracking
                    "disableMetricTracking": false, //Turns on/off custom implementation of metric tracking
                    "disableEventTracking": false //Turns on/off custom implementation of event tracking
               }
            */

            if (config.hasOwnProperty("enableDebug")) { //default false
                enableDebug = config.enableDebug;
                (window as any).appInsights.config.enableDebug = config.enableDebug;
            }

            if (config.hasOwnProperty("disableTelemetry")) //default false
                (window as any).appInsights.config.disableTelemetry = config.disableTelemetry;

            if (config.hasOwnProperty("verboseLogging")) //default false
                (window as any).appInsights.config.verboseLogging = config.verboseLogging;

            if (config.hasOwnProperty("disableExceptionTracking")) //default false
                (window as any).appInsights.config.disableExceptionTracking = config.disableExceptionTracking;

            if (config.hasOwnProperty("disableAjaxTracking")) //default false
                (window as any).appInsights.config.disableAjaxTracking = config.disableAjaxTracking;

            if (config.hasOwnProperty("maxAjaxCallsPerView")) //default 500, -1 = all
                (window as any).appInsights.config.maxAjaxCallsPerView = config.maxAjaxCallsPerView;

        } catch (error) {
            console.log(`Application Insights error parsing configuration parameters: ${error}`);
        }
    }

    function trackPageView(formName: string, props: any) {

        if (isNaN(targetPage.performance.timing.loadEventEnd) || isNaN(targetPage.performance.timing.responseEnd) ||
            targetPage.performance.timing.loadEventEnd === 0 || targetPage.performance.timing.responseEnd === 0) {
            setTimeout(() => {
                trackPageView(formName, props);
            }, 50);
        } else {
            var pageLoad = targetPage.performance.timing.loadEventEnd - targetPage.performance.timing.responseEnd;

            (window as any).appInsights.trackPageView(formName, null, props, null, pageLoad);
            if (enableDebug) {
                console.log("Application Insights page tracking started");
                console.log(`Application Insights logged page load time: ${pageLoad}ms`);
            }
        }
    }

    export function writeEvent(name: string, newProps: any, measurements: any) {
        if (disableTelemetry || disableEventTracking)
            return;

        (window as any).appInsights.trackEvent(name, combineProps(props, newProps), measurements);
        if (enableDebug)
            console.log(`Application Insights logged event: ${name}`);
    }

    export function writeMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, newProps?: any) {
        if (disableTelemetry || disableMetricTracking)
            return;

        (window as any).appInsights.trackMetric(name, average, sampleCount, min, max, combineProps(props, newProps));
        if (enableDebug)
            console.log(`Application Insights logged metric: ${name}`);
    }

    export function writeException(exception: Error, handledAt?: string, newProps?: any, measurements?: any, severityLevel?: AI.SeverityLevel) {
        if (disableTelemetry || disableExceptionTracking)
            return;

        (window as any).appInsights.trackException(exception, handledAt, combineProps(props, newProps), measurements, severityLevel);
        if (enableDebug)
            console.log(`Application Insights logged exception: ${exception.name}`);
    }

    export function writeTrace(message: string, newProps?: any, severityLevel?: AI.SeverityLevel) {
        if (disableTelemetry || disableTraceTracking)
            return;

        (window as any).appInsights.trackTrace(message, combineProps(props, newProps), severityLevel);
        if (enableDebug)
            console.log(`Application Insights logged trace: ${message}`);
    }

    export function writeDependency(id: string, method: string, absoluteUrl: string, pathName: string, totalTime: number, success: boolean, resultCode: number, newProps?: any) {
        if (disableTelemetry || disableDependencyTracking)
            return;

        (window as any).appInsights.trackDependency(id, method, absoluteUrl, pathName, totalTime, success, resultCode, combineProps(props, newProps), null);
        if (enableDebug)
            console.log(`Application Insights logged dependency: ${id}: ${totalTime}`);
    }

    export function writeMethodTime(methodName: string, start: number, end: number) {
        const time = end - start;
        writeMetric(`Method Time: ${methodName}`, time, null, null, null);
        if (enableDebug)
            console.log(`Application Insights logged method time: ${methodName}: ${time}ms`);
    }

    export function trackDependencyTime(req: any, methodName: string) {
        // ReSharper disable once Html.EventNotResolved
        req.addEventListener("loadstart", () => {
            getStartTime(req, methodName);
        });

        req.addEventListener("load", () => {
            getEndTime(req, true);
        });

        req.addEventListener("error", () => {
            getEndTime(req, false);
        });
    }

    export function getStartTime(req: any, methodName: string) {
        req.t0 = performance.now();
        req.methodName = methodName;
    }

    export function getEndTime(req: any, success: boolean) {
        writeDependency(Microsoft.ApplicationInsights.Util.newId(), req._method, req._url, `${req._url}`,
            performance.now() - req.t0, success, req.status, { methodName: req.methodName, mode: getMode(req._async) });
    }

    function combineProps(props: any, newProps: any) {
        if (newProps === null)
            return props;

        for (let attrname in newProps) {
            if (newProps.hasOwnProperty(attrname))
                props[attrname] = newProps[attrname];
        }

        return props;
    }

    function getFormTypeName(formType: number): string {
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

    function getMode(mode: boolean): string {
        return (mode) ? "Asynchronous" : "Synchronous";
    }

}

//https://stackoverflow.com/questions/5202296/add-a-hook-to-all-ajax-requests-on-a-page

var xhrProto = XMLHttpRequest.prototype,
    origOpen = xhrProto.open;

xhrProto.open = function (method, url, async) {
    this._url = url;
    this._method = method;
    this._async = async;
    return origOpen.apply(this, arguments);
};