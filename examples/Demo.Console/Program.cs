using Coldairarrow.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        public static void GetIdTest()
        {
            new IdHelperBootstrapper()
                .UseZookeeper("127.0.0.1:2181", 5000, "Test")
                .Boot();

            Console.WriteLine($"WorkerId:{IdHelper.WorkerId}");
            while (true)
            {
                try
                {
                    //IdHelper.GetId();
                    Console.WriteLine($"WorkerId:{IdHelper.WorkerId},Id:{IdHelper.GetId()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
            //List<Task> tasks = new List<Task>();
            //BlockingCollection<string> ids = new BlockingCollection<string>();
            //for (int i = 0; i < 4; i++)
            //{
            //    tasks.Add(Task.Run(() =>
            //    {
            //        for (int j = 0; j < 1000000; j++)
            //        {
            //            ids.Add(IdHelper.GetId());
            //        }
            //    }));
            //}
            //Task.WaitAll(tasks.ToArray());
            //Console.WriteLine($"是否有重复:{ids.Count != ids.Distinct().Count()}");
        }
        static void Main(string[] args)
        {
            GetIdTest();
            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
