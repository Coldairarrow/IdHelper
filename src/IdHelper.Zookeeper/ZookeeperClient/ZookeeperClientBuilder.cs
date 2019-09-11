using org.apache.zookeeper;
using System;
using System.Diagnostics;
using System.Threading;
using static org.apache.zookeeper.Watcher.Event;

namespace Coldairarrow.Util
{
    internal class ZookeeperClientBuilder
    {
        public ZookeeperClientBuilder(string connectString, int sessionTimeout)
        {
            _connectString = connectString;
            _sessionTimeout = sessionTimeout;
        }
        private string _connectString { get; }
        private int _sessionTimeout { get; }
        private Action<ZooKeeper, WatchedEvent> _onEvent { get; set; }
        private Action<ZooKeeper> _onConnected { get; set; }
        Action<TraceLevel, string, Exception> _logHandler { get; set; }
        private ManualResetEvent _connectedReset = new ManualResetEvent(false);

        public ZookeeperClientBuilder OnEvent(Action<ZooKeeper, WatchedEvent> action)
        {
            _onEvent = action;

            return this;
        }

        public ZookeeperClientBuilder OnConnected(Action<ZooKeeper> action)
        {
            _onConnected = action;

            return this;
        }

        public ZookeeperClientBuilder HandleLog(Action<TraceLevel, string, Exception> action)
        {
            _logHandler = action;

            return this;
        }

        public ZooKeeper Build()
        {
            ZooKeeper.CustomLogConsumer = new ZookeeperClientLogConsumer(_logHandler);
            ZooKeeper.LogToFile = false;
            ZooKeeper.LogToTrace = true;
            ZooKeeper.LogLevel = TraceLevel.Error;
            var watcher = new ZookeeperClientWatcher();
            var theClient = new ZooKeeper(_connectString, _sessionTimeout, watcher);
            watcher.EventHandler = theEvent =>
            {
                if (theEvent.get_Type() == EventType.None && theEvent.getState() == KeeperState.SyncConnected)
                    _connectedReset.Set();

                _connectedReset.WaitOne();
                _onEvent?.Invoke(theClient, theEvent);
            };

            bool connected = _connectedReset.WaitOne(5 * 1000);
            if (connected == false)
                throw new Exception("连接Zookeeper服务器超时!");
            _onConnected?.Invoke(theClient);

            return theClient;
        }
    }
}
