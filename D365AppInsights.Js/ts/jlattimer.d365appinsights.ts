/// <reference path="../node_modules/@types/xrm/index.d.ts" />
/// <reference path="../scripts/ai.1.0.18-build01843.d.ts" />

namespace AiFormLogger {
    declare var impersonatingUserId: string;
    declare var enableDebug: boolean;
    declare var props: Object;
    declare var disablePageviewTracking: boolean;
    declare var disablePageLoadTimeTracking: boolean;
    declare var disableTraceTracking: boolean;
    declare var disableExceptionTracking: boolean;
    declare var disableDependencyTracking: boolean;
    declare var disableMetricTracking: boolean;
    declare var disableEventTracking: boolean;
    declare var targetPage: any;
    enableDebug = false;
    disableTraceTracking = false;
    disablePageviewTracking = false;
    disablePageLoadTimeTracking = false;
    disableExceptionTracking = false;
    disableDependencyTracking = false;
    disableMetricTracking = false;
    disableEventTracking = false;
    targetPage = window;

    export function startLogging(config?: any) {
        if (config)
            setConfigOptions(config);

        // Capture PageView start
        if (!disablePageviewTracking)
            var pageViewStart = performance.now();

        if (/ClientApiWrapper\.aspx/i.test(window.location.pathname)) {
            targetPage = window.parent;
            if (enableDebug)
                console.log("Application Insights page target: window.parent");
        }

        var formName = Xrm.Page.ui.formSelector.getCurrentItem().getLabel();

        // Custom implementation of Pageview to avoid duplicate events being 
        // recorded likely due to CRM already implementing AI which currently
        // has poor support for multiple AI accounts
        if (!disablePageviewTracking) {
            (window as any).addEventListener("beforeunload",
                () => {
                    const envelope = createPageViewEnvelope(formName, pageViewStart);

                    if (navigator.sendBeacon) {
                        navigator.sendBeacon((window as any).appInsights.config.endpointUrl, JSON.stringify(envelope));
                        if (enableDebug)
                            console.log("Application Insights logged Pageview via Beacon");
                    } else {
                        // IE doesn't support Beacon - use sync XHR w/ delay instead
                        // Need slight delay to ensure PageView gets sent
                        var waitMs = 100; // Miliseconds wait
                        var futureTime = (new Date()).getTime() + waitMs;

                        sendPageViewRequest(envelope);

                        // Delay
                        while ((new Date()).getTime() < futureTime) { }
                    }
                },
                false);
        }

        props = {};
        props["entityId"] = Xrm.Page.data.entity.getId().substr(1, 36);
        props["entityName"] = Xrm.Page.data.entity.getEntityName();
        props["formType"] = getFormTypeName(Xrm.Page.ui.getFormType());
        props["formName"] = formName;
        props["orgName"] = Xrm.Page.context.getOrgUniqueName();
        props["orgVersion"] = Xrm.Page.context.getVersion();
        props["source"] = "JavaScript";

        if (!(window as any).appInsights.queue)
            (window as any).appInsights.queue = [];

        (window as any).appInsights.queue.push(() => {
            (window as any).appInsights.context.addTelemetryInitializer(envelope => {
                const telemetryItem = envelope.data.baseData;
                // Add CRM specific properties to every request
                telemetryItem.properties = combineProps(telemetryItem.properties, props);
            });
        });

        (window as any).appInsights.setAuthenticatedUserContext(Xrm.Page.context.getUserId().substr(1, 36), null, false);

        writePageLoadMetric();
    }

    function setConfigOptions(config: any) {
        try {
            if (config.hasOwnProperty("enableDebug")) { //default false
                enableDebug = config.enableDebug;
                (window as any).appInsights.config.enableDebug = config.enableDebug;
            }

            if (config.hasOwnProperty("disablePageviewTracking")) //default false
                disablePageviewTracking = config.disablePageviewTracking;

            if (config.hasOwnProperty("disablePageLoadTimeTracking")) //default false
                disablePageLoadTimeTracking = config.disablePageLoadTimeTracking;

            if (config.hasOwnProperty("disableExceptionTracking")) { //default false
                disableExceptionTracking = config.disableExceptionTracking;
                (window as any).appInsights.config.disableExceptionTracking = config.disableExceptionTracking;
            }

            if (config.hasOwnProperty("disableAjaxTracking")) //default false
                (window as any).appInsights.config.disableAjaxTracking = config.disableAjaxTracking;

            if (config.hasOwnProperty("maxAjaxCallsPerView")) //default 500, -1 = all
                (window as any).appInsights.config.maxAjaxCallsPerView = config.maxAjaxCallsPerView;

            if (config.hasOwnProperty("disableTraceTracking")) { //default false
                disableTraceTracking = config.disableTraceTracking;
                (window as any).appInsights.config.disableTraceTracking = config.disableTraceTracking;
            }

            if (config.hasOwnProperty("disableDependencyTracking")) { //default false
                disableDependencyTracking = config.disableDependencyTracking;
                (window as any).appInsights.config.disableDependencyTracking = config.disableDependencyTracking;
            }

            if (config.hasOwnProperty("disableMetricTracking")) { //default false
                disableMetricTracking = config.disableMetricTracking;
                (window as any).appInsights.config.disableMetricTracking = config.disableMetricTracking;
            }

            if (config.hasOwnProperty("disableEventTracking")) { //default false
                disableEventTracking = config.disableEventTracking;
                (window as any).appInsights.config.disableEventTracking = config.disableEventTracking;
            }

        } catch (error) {
            console.log(`Application Insights error parsing configuration parameters: ${error}`);
        }
    }

    function writePageLoadMetric() {
        if (disablePageLoadTimeTracking)
            return;

        if (isNaN(targetPage.performance.timing.loadEventEnd) || isNaN(targetPage.performance.timing.responseEnd) ||
            targetPage.performance.timing.loadEventEnd === 0 || targetPage.performance.timing.responseEnd === 0) {
            setTimeout(() => {
                writePageLoadMetric();
            }, 50);
        } else {
            const pageLoad = targetPage.performance.timing.loadEventEnd - targetPage.performance.timing.responseEnd;

            writeMetric("PageLoad", pageLoad, 1, null, null, null);
            if (enableDebug) {
                console.log(`Application Insights logged metric: PageLoad time: ${pageLoad}ms`);
            }
        }
    }

    export function writeEvent(name: string, newProps: any, measurements: any) {
        if (disableEventTracking)
            return;

        (window as any).appInsights.trackEvent(name, newProps, measurements);
        if (enableDebug)
            console.log(`Application Insights logged event: ${name}`);
    }

    export function writeMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, newProps?: any) {
        if (disableMetricTracking)
            return;

        (window as any).appInsights.trackMetric(name, average, sampleCount, min, max, newProps);
        if (enableDebug)
            console.log(`Application Insights logged metric: ${name}`);
    }

    export function writeException(exception: Error, handledAt?: string, newProps?: any, measurements?: any, severityLevel?: AI.SeverityLevel) {
        if (disableExceptionTracking)
            return;

        (window as any).appInsights.trackException(exception, handledAt, newProps, measurements, severityLevel);
        if (enableDebug)
            console.log(`Application Insights logged exception: ${exception.name}`);
    }

    export function writeTrace(message: string, newProps?: any, severityLevel?: AI.SeverityLevel) {
        if (disableTraceTracking)
            return;

        (window as any).appInsights.trackTrace(message, newProps, severityLevel);
        if (enableDebug)
            console.log(`Application Insights logged trace: ${message}`);
    }

    export function writeDependency(id: string, method: string, absoluteUrl: string, pathName: string, totalTime: number, success: boolean, resultCode: number, newProps?: any) {
        if (disableDependencyTracking)
            return;

        (window as any).appInsights.trackDependency(id, method, absoluteUrl, pathName, totalTime, success, resultCode, newProps, null);
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

    function getStartTime(req: any, methodName: string) {
        req.t0 = performance.now();
        req.methodName = methodName;
    }

    function getEndTime(req: any, success: boolean) {
        writeDependency(Microsoft.ApplicationInsights.Util.newId(), req._method, req._url, `${req._url}`,
            performance.now() - req.t0, success, req.status, { methodName: req.methodName, mode: getMode(req._async) });
    }

    function combineProps(props: any, newProps: any) {
        if (!props && !newProps)
            return null;
        if (newProps === null)
            return props;
        if (props === null)
            return newProps;

        for (let attrname in newProps) {
            if (newProps.hasOwnProperty(attrname))
                props[attrname] = newProps[attrname];
        }

        return props;
    }

    function getIdFromCookie(cookieName: string) {
        const cookie = Microsoft.ApplicationInsights.Util.getCookie(cookieName);
        if (!cookie)
            return null;
        const params = cookie.split(Microsoft.ApplicationInsights.Context.User.cookieSeparator);
        if (params.length < 1)
            return null;

        return params[0];
    }

    function sendPageViewRequest(envelope: Microsoft.Telemetry.Envelope): void {
        var req = new XMLHttpRequest();
        req.open("POST", (window as any).appInsights.config.endpointUrl, false); // Doesn't work if async
        req.setRequestHeader("Accept", "*/*");
        req.setRequestHeader("Content-Type", "application/json");
        req.onreadystatechange = () => {
            if (req.readyState === 4) {
                if (req.status === 200) {
                    if (enableDebug)
                        console.log("Application Insights logged Pageview via XHR");
                }
            }
        }
        req.send(JSON.stringify(envelope));
    }

    function createPageViewEnvelope(formName: string, pageViewStart: number): Microsoft.Telemetry.Envelope {
        var iKey = (window as any).appInsights.config.instrumentationKey;
        var envelope = new Microsoft.Telemetry.Envelope;
        envelope.time = new Date().toISOString();
        envelope.iKey = iKey;
        envelope.name = `Microsoft.ApplicationInsights.${iKey.replace("-", "")}.Pageview`;

        envelope.data = { baseType: "PageviewData" };
        envelope.tags["ai.session.id"] = getIdFromCookie("ai_session");
        envelope.tags["ai.device.id"] = (window as any).appInsights.context.device.id;
        envelope.tags["ai.device.type"] = (window as any).appInsights.context.device.type;
        envelope.tags["ai.internal.sdkVersion"] = (window as any).appInsights.context.internal.sdkVersion;
        envelope.tags["ai.user.id"] = getIdFromCookie(Microsoft.ApplicationInsights.Context.User.userCookieName);
        envelope.tags["ai.user.authUserId"] = (window as any).appInsights.context.user.authenticatedId.toUpperCase();
        envelope.tags["ai.operation.id"] = (window as any).appInsights.context.operation.id;
        envelope.tags["ai.operation.name"] = (window as any).appInsights.context.operation.name;
        envelope.data.baseType = "PageviewData";
        var pageViewData = new AI.PageViewData;
        pageViewData.ver = 2;
        pageViewData.name = formName;
        pageViewData.url = Microsoft.ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeUrl((window as any).location.href);
        var d = performance.now() - pageViewStart;
        pageViewData.duration = Microsoft.ApplicationInsights.Util.msToTimeSpan(d);
        envelope.data["baseData"] = pageViewData;
        envelope.data["baseData"]["properties"] = Microsoft.ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeProperties(props);
        envelope.data["baseData"]["measurements"] = null;
        envelope.data["baseData"]["id"] = Microsoft.ApplicationInsights.Util.newId();

        return envelope;
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

    var xhrProto = XMLHttpRequest.prototype,
        origOpen = xhrProto.open;

    xhrProto.open = function (method, url, async) {
        this._url = url;
        this._method = method;
        this._async = async;
        return origOpen.apply(this, arguments);
    };
}