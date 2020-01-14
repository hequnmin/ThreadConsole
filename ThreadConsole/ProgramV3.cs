//https://blog.csdn.net/kingshown_WZ/article/details/89074039

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace ThreadConsole
{
    class Program
    {
        //定义一个委托类
        private delegate UserInfo MyDelegate(UserInfo userInfo);

        //static List<UserInfo> userInfoList = new List<UserInfo>();

        static void Main(string[] args)
        {
            ThreadDemoClass demoClass = new ThreadDemoClass();
            UserInfo userInfo = null;

            //创建一个委托并绑定方法
            MyDelegate myDelegate = new MyDelegate(demoClass.Run);

            //创建一个回调函数的委托
            AsyncCallback asyncCallback = new AsyncCallback(Complete);

            //回调函数的参数
            string str = "I'm the parameter of the callback function!";

            for (int i = 0; i < 3; i++)
            {
                userInfo = new UserInfo();
                userInfo.Name = "Brambling" + i.ToString();
                userInfo.Age = 33 + i;

                //传入参数并执行异步委托，并设置回调函数
                IAsyncResult result = myDelegate.BeginInvoke(userInfo, asyncCallback, str);
            }

            Console.WriteLine("Main thread working...");
            Console.WriteLine("Main thread ID is:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine();

            Console.ReadKey();
        }

        public static void Complete(IAsyncResult result)
        {
            UserInfo userInfoRes = null;

            AsyncResult asyncResult = (AsyncResult)result;

            //获取在其上调用异步调用的委托对象
            MyDelegate myDelegate = (MyDelegate)asyncResult.AsyncDelegate;

            //结束在其上调用的异步委托，并获取返回值
            userInfoRes = myDelegate.EndInvoke(result);

            Console.WriteLine("My name is " + userInfoRes.Name);
            Console.WriteLine("I'm " + userInfoRes.Age + " years old this year");
            Console.WriteLine("Thread ID is:" + userInfoRes.ThreadId);

            //获取回调函数的参数
            string str = result.AsyncState as string;
            Console.WriteLine(str);
        }
    }

    public class ThreadDemoClass
    {
        public UserInfo Run(UserInfo userInfo)
        {
            userInfo.ThreadId = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine("Child thread working...");
            Console.WriteLine("Child thread ID is:" + userInfo.ThreadId);
            Console.WriteLine($"UserInfo Name:{userInfo.Name} Age:{userInfo.Age}");
            Console.WriteLine();

            return userInfo;
        }
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int ThreadId { get; set; }
    }

}
