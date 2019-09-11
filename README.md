# 目录
- [简介](#简介)
- [产生背景](#产生背景)
- [使用方式](#使用方式)
	- [原始版](#原始版)
	- [完美版](#完美版)
	- [测试](#测试)
- [结尾](#结尾)

# 简介
IdHelper是一个.NET（支持.NET45+或.NET Standard2+）生成分布式趋势自增Id组件，有两个版本：**原始版**为基于雪花Id（不了解请自行百度）方案，需要手动管理设置WorkerId；**完美版**在原始版的基础上使用Zookeeper来解决原始版中的WorkerId的分配问题和时间回拨问题。

原始版安装方式：Nuget安装**IdHelper**即可

完美版安装方式：Nuget安装**IdHelper.Zookeeper**即可

请按需选择，强烈推荐**完美版**

项目地址：https://github.com/Coldairarrow/IdHelper

# 产生背景
分布式趋势自增Id的生成方案比较多，其中雪花Id是比较常用的，但是雪花Id及其依赖WorkerId的分配和机器时钟。WorkerId分配问题：传统雪花Id是需要分配数据中心Id和机器Id（即WorkerId），我为了使用方便（项目比较小），用不到数据中心Id，就把数据中心Id去掉并补充到机器Id，使机器Id可分配范围为1~1023，每个服务机器Id不能重复，若手工去为每个服务设置无疑十分麻烦还容易搞错（其实是**懒**）。时钟回拨问题：由于强依赖机器时钟，因此当时间回拨时将发生灾难性问题，虽然这种概率很小，但是实际存在。为了解决上述两个问题，本组件应运而生。

# 使用方式

## 原始版

Nuget安装包：IdHelper

刚出炉的包，排名比较靠后，请认准作者：Coldairarrow

``` c#
using Coldairarrow.Util;
using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            new IdHelperBootstrapper()
                //设置WorkerId
                .SetWorkderId(1)
                .Boot();

            Console.WriteLine($"WorkerId:{IdHelper.WorkerId},Id:{IdHelper.GetId()}");

            Console.ReadLine();
        }
    }
}

```
## 完美版
1：安装并配置JAVA环境（Zookeeper需要用JAVA）  教程：[连接](https://blog.csdn.net/qq_42040731/article/details/82598034)
2：安装并启动Zookeeper，教程：[链接](https://blog.csdn.net/ring300/article/details/80446918)  
3：Nuget安装包：IdHelper.Zookeeper  

``` c#
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

```
## 测试
``` c#
using Coldairarrow.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string conString = "127.0.0.1:2181";
            new IdHelperBootstrapper()
                .UseZookeeper(conString, 200, "Test")
                .Boot();

            Console.WriteLine($"WorkerId:{IdHelper.WorkerId}");

            Stopwatch watch = new Stopwatch();
            watch.Start();
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
            watch.Stop();
            Console.WriteLine($"耗时:{watch.ElapsedMilliseconds}ms,是否有重复:{ids.Count != ids.Distinct().Count()}");
        }
    }
}

```


# 结尾
以上所有示例在源码中都有，若觉得不错请点赞加星星，希望能够帮助到大家。

有任何问题请及时反馈或加群交流

QQ群1:（已满） 

QQ群2:579202910
