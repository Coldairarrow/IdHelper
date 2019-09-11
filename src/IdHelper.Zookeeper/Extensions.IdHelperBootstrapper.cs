namespace Coldairarrow.Util
{
    /// <summary>
    /// 拓展类
    /// </summary>
    public static partial class IdHelperBootstrapperExtention
    {
        /// <summary>
        /// 使用Zookeeper
        /// </summary>
        /// <param name="bootstrapper">原引导着</param>
        /// <param name="connectString">Zookeeper连接字符串</param>
        /// <param name="sessionTimeout">超时时间</param>
        /// <param name="projectKey">项目主键,同一个项目下的WokerId范围:1~1023</param>
        /// <returns></returns>
        public static IdHelperBootstrapper UseZookeeper(this IdHelperBootstrapper bootstrapper, string connectString, int sessionTimeout, string projectKey)
        {
            return new ZookeeperBootstrapper(connectString, sessionTimeout, projectKey);
        }
    }
}
