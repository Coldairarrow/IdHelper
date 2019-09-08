namespace Coldairarrow.Util
{
    /// <summary>
    /// Id获取帮助类
    /// </summary>
    public static class IdHelper
    {
        internal static IdWorker IdWorker { get; set; }

        /// <summary>
        /// 当前WorkerId
        /// </summary>
        public static long WorkerId { get => IdWorker.WorkerId; }

        /// <summary>
        /// 获取String型雪花Id
        /// </summary>
        /// <returns></returns>
        static public string GetId()
        {
            return IdWorker.NextId().ToString();
        }

        /// <summary>
        /// 获取long型雪花Id
        /// </summary>
        /// <returns></returns>
        static public long GetLongId()
        {
            return IdWorker.NextId();
        }

        /// <summary>
        /// 获取雪花Id
        /// </summary>
        /// <returns></returns>
        static public SnowflakeId GetStructId()
        {
            return new SnowflakeId(IdWorker.NextId());
        }
    }
}
