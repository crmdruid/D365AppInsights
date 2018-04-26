using System;

namespace D365AppInsights.Workflow
{
    public interface ITimeProvider
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }

        DateTime Today { get; }
    }
}