using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine("Main threadId is:" + Thread.CurrentThread.ManagedThreadId);

            //Thread thread = new Thread(new ThreadStart(AsyncThread));
            //thread.IsBackground = true;
            //thread.Start();
            //thread.Join();

            ThreadDemoClass demoClass = new ThreadDemoClass();

            //设置线程池按需创建的线程的最小数量
            //第一个参数为由线程池根据需要创建的新的最小工作程序线程数
            //第二个参数为异步 I/O 线程数
            ThreadPool.SetMinThreads(5, 5);

            //设置同时处于活动状态的线程池的线程数，所有大于次数目的请求将保持排队状态，直到线程池变为可用
            //第一个参数为线程池中辅助线程的最大数目
            //第二个参数为异步 I/O 线程数
            ThreadPool.SetMaxThreads(100, 100);

            //使用委托绑定线程池要执行的方法（无参数）
            WaitCallback waitCallback1 = new WaitCallback(demoClass.Run1);
            //将方法排入队列，在线程池变为可用时执行
            ThreadPool.QueueUserWorkItem(waitCallback1);

            //使用委托绑定线程池要执行的方法（有参数）
            WaitCallback waitCallback2 = new WaitCallback(demoClass.Run1);
            //将方法排入队列，在线程池变为可用时执行
            ThreadPool.QueueUserWorkItem(waitCallback2, "Brambling");

            UserInfo userInfo = new UserInfo();
            userInfo.Name = "Brambling";
            userInfo.Age = 33;
            //使用委托绑定线程池要执行的方法（自定义类型的参数）
            WaitCallback waitCallback3 = new WaitCallback(demoClass.Run2);
            //将方法排入队列，在线程池变为可用时执行
            ThreadPool.QueueUserWorkItem(waitCallback3, userInfo);

            Console.WriteLine();
            Console.WriteLine("Main thread working...");
            Console.WriteLine("Main thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.ReadKey();

        }

        //以异步方式调用
        static void AsyncThread()
        {
            try
            {
                string message = string.Format("\nAsync threadId is:{0}", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine(message);

                for (int n = 0; n < 10; n++)
                {
                    if (n >= 4) //当n等于4时，终止线程
                    {
                        Thread.CurrentThread.Abort(n);
                    }
                    Thread.Sleep(300);
                    Console.WriteLine("The number is:" + n.ToString());
                }
            }
            catch (ThreadAbortException ex)
            {
                //输出终止线程时n的值
                if (ex.ExceptionState != null)
                    Console.WriteLine(string.Format("Thread abort when the number is: {0}!",
                                                     ex.ExceptionState.ToString()));

                Thread.ResetAbort(); //取消终止，继续执行线程
                Console.WriteLine("Thread ResetAbort!");
                Console.WriteLine("Thread Close!"); //线程结束
                Console.ReadKey();
            }
        }
    }

    public class ThreadDemoClass
    {
        public void Run(object obj)
        {
            UserInfo userInfo = (UserInfo)obj;

            Console.WriteLine("Child thread working...");
            Console.WriteLine("My name is " + userInfo.Name);
            Console.WriteLine("I'm " + userInfo.Age + " years old this year");
            Console.WriteLine("Child thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());
        }

        public void Run1(object obj)
        {
            string name = obj as string;

            Console.WriteLine();
            Console.WriteLine("Child thread working...");
            Console.WriteLine("My name is " + name);
            Console.WriteLine("Child thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());
        }

        public void Run2(object obj)
        {
            UserInfo userInfo = (UserInfo)obj;

            Console.WriteLine();
            Console.WriteLine("Child thread working...");
            Console.WriteLine("My name is " + userInfo.Name);
            Console.WriteLine("I'm " + userInfo.Age + " years old this year");
            Console.WriteLine("Child thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());
        }

    }

    public class UserInfo
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

}
