using org.apache.zookeeper;
using System;
using System.Threading.Tasks;

namespace Coldairarrow.Util
{
    internal class ZookeeperClientWatcher : Watcher
    {
        public Action<WatchedEvent> EventHandler { get; set; }
        public override async Task process(WatchedEvent @event)
        {
            await Task.Run(() =>
            {
                EventHandler?.Invoke(@event);
            });
        }
    }
}
