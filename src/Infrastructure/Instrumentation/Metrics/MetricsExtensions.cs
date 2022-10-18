using System.Collections.Generic;
using System.Linq;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;

namespace Infrastructure.Instrumentation.Metrics
{
    public static class MetricsExtensions
    {
        public static readonly TimerOptions IODurationTimerOptions = new TimerOptions
        {
            Name = "IO Duration Seconds",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Seconds
        };

        public static readonly TimerOptions OperationDurationTimerOptions = new TimerOptions
        {
            Name = "Operation Duration Seconds",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Seconds
        };

        public static readonly CounterOptions IOCounterOptions = new CounterOptions
        {
            Name = "IO Operation Total",
            MeasurementUnit = Unit.Items,
        };

        public static readonly CounterOptions OperationCounterOptions = new CounterOptions
        {
            Name = "Operation Total",
            MeasurementUnit = Unit.Items,
        };

        public static TimerContext TimeIO(this IMetrics metrics, string type, string resource, string operation, IDictionary<string, string> tags = null)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"type", type},
                {"resource", resource},
                {"operation", operation}
            };

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            return metrics.Measure.Timer.Time(IODurationTimerOptions, metricTags);
        }

        public static TimerContext TimeOperation(this IMetrics metrics, string operation, string component = null, IDictionary<string, string> tags = null)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"operation", operation}
            };

            if (!string.IsNullOrWhiteSpace(component))
            {
                tagsDict.Add("component", component);
            }

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            return metrics.Measure.Timer.Time(OperationDurationTimerOptions, metricTags);
        }

        public static void IncrementIO(
            this IMetrics metrics,
            string type,
            string resource,
            string operation,
            IDictionary<string, string> tags = null,
            long incrementBy = 1)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"type", type},
                {"resource", resource},
                {"operation", operation}
            };

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            metrics.Measure.Counter.Increment(IOCounterOptions, metricTags, incrementBy);
        }

        public static void DecrementIO(
            this IMetrics metrics,
            string type,
            string resource,
            string operation,
            IDictionary<string, string> tags = null,
            long decrementBy = 1)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"type", type},
                {"resource", resource},
                {"operation", operation}
            };

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            metrics.Measure.Counter.Decrement(IOCounterOptions, metricTags, decrementBy);
        }

        public static void IncrementOperation(
            this IMetrics metrics,
            string operation,
            string component = null,
            IDictionary<string, string> tags = null,
            long incrementBy = 1)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"operation", operation}
            };

            if (!string.IsNullOrWhiteSpace(component))
            {
                tagsDict.Add("component", component);
            }

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            metrics.Measure.Counter.Increment(OperationCounterOptions, metricTags, incrementBy);
        }

        public static void DecrementOperation(
            this IMetrics metrics,
            string operation,
            string component = null,
            IDictionary<string, string> tags = null,
            long decrementBy = 1)
        {
            var tagsDict = new Dictionary<string, string>
            {
                {"operation", operation}
            };

            if (!string.IsNullOrWhiteSpace(component))
            {
                tagsDict.Add("component", component);
            }

            if (tags != null)
            {
                foreach (var (key, value) in tags)
                {
                    tagsDict.TryAdd(key, value);
                }
            }

            var metricTags = new MetricTags(tagsDict.Keys.ToArray(), tagsDict.Values.ToArray());

            metrics.Measure.Counter.Decrement(OperationCounterOptions, metricTags, decrementBy);
        }
    }
}