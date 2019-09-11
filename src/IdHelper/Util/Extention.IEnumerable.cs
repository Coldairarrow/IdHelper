using System;
using System.Collections.Generic;
using System.Linq;

namespace Coldairarrow.Util
{
    internal static partial class Extention
    {
        /// <summary>
        /// 复制序列中的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="iEnumberable">原数据</param>
        /// <param name="startIndex">原数据开始复制的起始位置</param>
        /// <param name="length">需要复制的数据长度</param>
        /// <returns></returns>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> iEnumberable, int startIndex, int length)
        {
            var sourceArray = iEnumberable.ToArray();
            T[] newArray = new T[length];
            Array.Copy(sourceArray, startIndex, newArray, 0, length);

            return newArray;
        }
    }
}
