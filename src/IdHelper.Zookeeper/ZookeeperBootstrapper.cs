using org.apache.zookeeper;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;
using static org.apache.zookeeper.ZooDefs;

namespace Coldairarrow.Util
{
    class ZookeeperBootstrapper : IdHelperBootstrapper
    {
        #region 构造函数

        public ZookeeperBootstrapper(string connectString, int sessionTimeout, string projectKey)
        {
            _connectString = connectString;
            _sessionTimeout = sessionTimeout;
            _projectKey = projectKey;
        }

        #endregion

        #region 私有成员

        private static ZooKeeper _zookeeperClient { get; set; }
        private string _connectString { get; }
        private int _sessionTimeout { get; }
        private string _projectKey { get; }
        private string _workderIdTmpRootPath { get => $"/{_projectKey}/IdHelper_WorkerIds_TMP"; }
        private string _workerIdRecordRootPath { get => $"/{_projectKey}/IdHelper_WorkerIds_Record"; }
        private async Task HandleEventAsync(ZooKeeper theClient, WatchedEvent theEvent)
        {
            string path = theEvent.getPath();
            var type = theEvent.get_Type();
            var state = theEvent.getState();
            Console.WriteLine($"收到事件,状态:{theEvent.getState()},类型:{type},节点:{path}");

            //Session过期需要重新建立Zookeeper客户端
            if (state == KeeperState.Expired)
            {
                BuildZookeeperClient();
                Boot();
                return;
            }
            //重新监听自己
            if (type != EventType.NodeDeleted && !string.IsNullOrEmpty(path))
                await theClient.existsAsync(theEvent.getPath(), true);

            //监听临时子节点
            await BindChildren(_workderIdTmpRootPath);

            //仅处理节点删除事件,记录停止时间,防止时间回调
            string pattern = $"^{_workderIdTmpRootPath}/(.*?)$";
            if (theEvent.get_Type() == Watcher.Event.EventType.NodeDeleted && Regex.IsMatch(path, pattern))
            {
                var match = Regex.Match(path, pattern);
                string deletedWorkerId = match.Groups[1].ToString();
                string recordNodePath = $"{_workerIdRecordRootPath}/{deletedWorkerId}";
                var data = Encoding.UTF8.GetString((await _zookeeperClient.getDataAsync(recordNodePath, true)).Data)
                    .ToObject<WorkerIdRecord>();
                data.EndTime = DateTime.Now;
                await _zookeeperClient.setDataAsync(recordNodePath, Encoding.UTF8.GetBytes(data.ToJson()));
            }
        }
        /// <summary>
        /// 同步临时节点与记录节点
        /// 说明:若临时节点不存在而记录节点存在,则将记录节点结束时间置为当前时间
        /// 若记录节点已结束10分钟(即最大允许10分钟时间回拨),则将记录节点也删除
        /// </summary>
        /// <returns></returns>
        private async Task SyncNodes()
        {
            var allRecordNodes = await _zookeeperClient.getChildrenAsync(_workerIdRecordRootPath, true);
            foreach (var aChild in allRecordNodes.Children)
            {
                string recordNodePath = $"{_workerIdRecordRootPath}/{aChild}";
                string tmpNodePath = $"{_workderIdTmpRootPath}/{aChild}";
                var data = Encoding.UTF8.GetString((await _zookeeperClient.getDataAsync(recordNodePath, true)).Data)
                    .ToObject<WorkerIdRecord>();
                if (data.EndTime < DateTime.Now.AddMinutes(-10))
                {
                    await _zookeeperClient.deleteAsync(recordNodePath);
                }
                //不存在对应临时节点
                else if (await _zookeeperClient.existsAsync(tmpNodePath, true) == null)
                {
                    data.EndTime = DateTime.Now;
                    await _zookeeperClient.setDataAsync(recordNodePath, Encoding.UTF8.GetBytes(data.ToJson()));
                }
            }
        }
        private async Task BindChildren(string parentPath)
        {
            var children = (await _zookeeperClient.getChildrenAsync(parentPath, true)).Children;
            foreach (var aChild in children)
            {
                await _zookeeperClient.existsAsync($"{parentPath}/{aChild}", true);
            }
        }
        private async Task CheckNodeExists(string path)
        {
            if (await _zookeeperClient.existsAsync(path, true) == null)
                await _zookeeperClient.createAsync(path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
        }
        private async Task<long> GetWorkerIdAsync()
        {
            if (_zookeeperClient == null)
                BuildZookeeperClient();
            //项目根节点
            await CheckNodeExists($"/{_projectKey}");
            //WorkerId记录根节点
            await CheckNodeExists(_workerIdRecordRootPath);
            //WorkerId临时根节点
            await CheckNodeExists(_workderIdTmpRootPath);
            //同步节点
            await SyncNodes();
            //分配WorkerId
            for (int i = 1; i < 1024; i++)
            {
                string recordNodePath = $"{_workerIdRecordRootPath}/{i}";
                string tmpNodePath = $"{_workderIdTmpRootPath}/{i}";
                if (await _zookeeperClient.existsAsync(recordNodePath, true) == null)
                {
                    await _zookeeperClient.createAsync(tmpNodePath, null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
                    WorkerIdRecord newData = new WorkerIdRecord
                    {
                        StartTime = DateTime.Now
                    };
                    await _zookeeperClient.createAsync(recordNodePath, Encoding.UTF8.GetBytes(newData.ToJson()), Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

                    return await Task.FromResult((long)i);
                }
            }
            throw new Exception("WorkerId已用完!");
        }
        private void BuildZookeeperClient()
        {
            if (_zookeeperClient != null)
                _zookeeperClient.closeAsync().GetAwaiter().GetResult();
            _zookeeperClient = new ZookeeperClientBuilder(_connectString, _sessionTimeout)
                    .OnEvent((theClient, theEvent) =>
                    {
                        HandleEventAsync(theClient, theEvent).GetAwaiter().GetResult();
                    })
                    .HandleLog((level, msg, ex) =>
                    {
                        Console.WriteLine($"消息:{msg},异常:{ex.GetType()}");
                    })
                    .Build();
        }

        #endregion

        #region 重写父类

        protected override long GetWorkerId()
        {
            return GetWorkerIdAsync().GetAwaiter().GetResult();
        }
        public override bool Available()
        {
            bool available = _zookeeperClient.getState() == ZooKeeper.States.CONNECTED;
            if (!available)
                throw new Exception("IdHelper.Zookeeper:与Zookeeper服务器断开!");
            else
                return available;
        }

        #endregion

        class WorkerIdRecord
        {
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        }
    }
}
