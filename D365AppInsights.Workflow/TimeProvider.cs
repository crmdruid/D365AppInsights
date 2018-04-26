using System;

namespace D365AppInsights.Workflow
{
    public static class TimeProvider
    {
        public static DateTime Now => _current.Now;

        public static DateTime UtcNow => _current.UtcNow;

        public static DateTime Today => _current.Today;

        internal static ITimeProvider Current
        {
            set => _current = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal static void ResetToDefault()
        {
            _current = SDefault;
        }

        private static readonly ITimeProvider SDefault = new DefaultTimeProvider();

        private static ITimeProvider _current = SDefault;
    }
}