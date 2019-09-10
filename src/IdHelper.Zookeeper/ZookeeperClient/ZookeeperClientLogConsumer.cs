using org.apache.utils;
using System;
using System.Diagnostics;

namespace Coldairarrow.Util
{
    class ZookeeperClientLogConsumer : ILogConsumer
    {
        public ZookeeperClientLogConsumer(Action<TraceLevel, string, Exception> action)
        {
            _logHandler = action;
        }
        Action<TraceLevel, string, Exception> _logHandler { get; }
        public void Log(TraceLevel severity, string className, string message, Exception exception)
        {
            _logHandler?.Invoke(severity, message, exception);
        }
    }
}
