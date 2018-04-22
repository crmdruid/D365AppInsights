using System;

namespace AppInsightsWorkflowLogger
{
    public interface ITimeProvider
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }

        DateTime Today { get; }
    }
}