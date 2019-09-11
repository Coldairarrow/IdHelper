using Coldairarrow.Util;
using System;

namespace Demo.Zookeeper
{
    class Program
    {
        static void Main(string[] args)
        {
            new IdHelperBootstrapper()
                //使用Zookeeper自动分配管理WorkerId,解决时间回退问题和自动分配问题
                .UseZookeeper("127.0.0.1:2181", 200, "Test")
                .Boot();

            Console.WriteLine($"WorkerId:{IdHelper.WorkerId},Id:{IdHelper.GetId()}");

            Console.ReadLine();
        }
    }
}
