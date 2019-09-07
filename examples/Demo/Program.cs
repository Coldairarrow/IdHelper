using Coldairarrow.Util;
using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            new IdHelperBootstrapper().SetWorkderId(3).Boot();

            Console.WriteLine(IdHelper.GetId());

            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
