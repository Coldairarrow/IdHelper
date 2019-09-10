using System;

namespace Coldairarrow.Util
{
    /// <summary>
    /// Id获取帮助类
    /// </summary>
    public static class IdHelper
    {
        internal static IdWorker IdWorker { get; set; }

        internal static IdHelperBootstrapper IdHelperBootstrapper { get; set; }

        /// <summary>
        /// 当前WorkerId,范围:1~1023
        /// </summary>
        public static long WorkerId { get => IdWorker.WorkerId; }

        /// <summary>
        /// 获取String型雪花Id
        /// </summary>
        /// <returns></returns>
        static public string GetId()
        {
            return GetLongId().ToString();
        }

        /// <summary>
        /// 获取long型雪花Id
        /// </summary>
        /// <returns></returns>
        static public long GetLongId()
        {
            if (!IdHelperBootstrapper.Available())
                throw new Exception("当前系统异常,无法生成Id,请检查相关配置");

            return IdWorker.NextId();
        }

        /// <summary>
        /// 获取雪花Id
        /// </summary>
        /// <returns></returns>
        static public SnowflakeId GetStructId()
        {
            return new SnowflakeId(GetLongId());
        }
    }
}
