using Coldairarrow.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            new IdHelperBootstrapper().SetWorkderId(3).Boot();

            Console.WriteLine($"WorkerId:{IdHelper.WorkerId}");
            List<Task> tasks = new List<Task>();
            BlockingCollection<string> ids = new BlockingCollection<string>();
            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 1000000; j++)
                    {
                        ids.Add(IdHelper.GetId());
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"是否有重复:{ids.Count != ids.Distinct().Count()}");

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
