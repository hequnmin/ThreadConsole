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
            ThreadDemoClass demoClass = new ThreadDemoClass();

            ////方式1:创建一个新的线程
            //Thread thread = new Thread(demoClass.Run);

            //方式2:创建一个委托，并把要执行的方法作为参数传递给这个委托
            //ThreadStart threadStart = new ThreadStart(demoClass.Run);
            //Thread thread = new Thread(threadStart);

            //方式3:创建一个委托，并把要执行的方法作为参数传递给这个委托
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(demoClass.Run);
            Thread thread = new Thread(threadStart);

            UserInfo userInfo = new UserInfo();
            userInfo.Name = "Brambling";
            userInfo.Age = 333;

            //设置为后台线程
            thread.IsBackground = true;

            ////方式1、2:
            ////开始线程
            //thread.Start();
            //方式3:开始线程，并传入参数
            thread.Start(userInfo);


            ////等待直到线程完成
            //thread.Join();


            Console.WriteLine("Main thread working...");
            Console.WriteLine("Main thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());//主线程
            Console.ReadKey();
        }
    }

    public class ThreadDemoClass
    {
        //方式1.2.
        //public void Run()
        //{
        //    Console.WriteLine("Child thread working...");
        //    Console.WriteLine("Child thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());//子线程
        //}

        //方式3.
        public void Run(object obj)
        {
            UserInfo userInfo = (UserInfo)obj;

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
