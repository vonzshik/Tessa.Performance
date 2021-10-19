using NLog;
using System;
using System.Diagnostics;
using Tessa.Extensions;

namespace Tessa.Performance
{
    public sealed class PerformanceExtensionTraceListener :
        ExtensionTraceListener
    {
        public const string TraceThresholdParamName = "TraceThreshold";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly int traceThreshold;

        public PerformanceExtensionTraceListener(int traceThreshold)
        {
            this.traceThreshold = traceThreshold;
        }

        public override void NotifyExecuting(IExtensionStrategyContext strategyContext)
        {
            if (strategyContext.ExecutionContext is ITraceableExtensionContext context)
            {
                context.EnableTracing = true;

                var stopwatch = new Stopwatch();
                strategyContext.TraceContext = stopwatch;
                stopwatch.Start();
            }
        }

        public override void NotifyExecuted(IExtensionStrategyContext strategyContext)
        {
            if (strategyContext.ExecutionContext is ITraceableExtensionContext context)
            {
                var stopwatch = (Stopwatch) strategyContext.TraceContext;
                stopwatch.Stop();
                strategyContext.TraceContext = null;

                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                if (elapsedMilliseconds < this.traceThreshold)
                {
                    return;
                }

                if (context.EnableTracing)
                {
                    var chainName = strategyContext.BuildKey.ExtensionType.Name;
                    var chainCode = context.GetHashCode();
                    var methodName = strategyContext.ExecutionKey.MethodName;
                    var className = strategyContext.ResolvedExtension.GetType().FullName;

                    var msg = new LogEventInfo(LogLevel.Trace, nameof(PerformanceExtensionTraceListener),
                        $"Extension performance trace, Elapsed: {elapsedMilliseconds}, MethodName: {methodName}, ClassName: {className}");
                    msg.Properties.Add("ChainName", chainName);
                    msg.Properties.Add("ChainCode", chainCode);
                    msg.Properties.Add("MethodName", methodName);
                    msg.Properties.Add("ClassName", className);
                    msg.Properties.Add("Elapsed", elapsedMilliseconds);
                    logger.Log(msg);
                }
            }
        }
    }
}
