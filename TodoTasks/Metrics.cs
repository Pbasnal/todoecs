using Prometheus;

namespace todorest
{
    public static class TodoMetrics
    {
        private static readonly Histogram methodDuration = Metrics
            .CreateHistogram("method_duration", "Histogram of methods.",
                labelNames: new[] { "method_name" });

        public static ITimer MethodMetrics(string methodName) => methodDuration
            .WithLabels(methodName).NewTimer();
    }
}