﻿//https://www.cnblogs.com/leslies2/archive/2012/02/08/2320914.html#t6

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;

using System.Net;
using System.Net.Sockets;

namespace ThreadConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            //设置CLR线程池最大线程数
            ThreadPool.SetMaxThreads(1000, 1000);

            //默认地址为127.0.0.1
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener tcpListener = new TcpListener(ipAddress, 501);
            tcpListener.Start();

            Console.WriteLine($"Listener: 127.0.0.1:501");
            Console.WriteLine();

            //以一个死循环来实现监听
            while (true)
            {   //调用一个ChatClient对象来实现监听
                ChatClient chatClient = new ChatClient(tcpListener.AcceptTcpClient());
            }
            //ChatClient chatClient = new ChatClient(tcpListener.AcceptTcpClient());
            //Console.WriteLine("TcpListener Start...");
            //Console.ReadKey();

        }
    }

    public class ChatClient
    {
        //解码
        enum enuEncoding { UTF8_GetString, UTF8_GetChars };
        enuEncoding RECEIVE_ENCODING = enuEncoding.UTF8_GetChars;

        static TcpClient tcpClient;
        static byte[] byteMessage;
        static string clientEndPoint;

        public ChatClient(TcpClient tcpClient1)
        {
            tcpClient = tcpClient1;
            tcpClient.ReceiveBufferSize = 1024;
            tcpClient.SendBufferSize = 1024;

            byteMessage = new byte[tcpClient.ReceiveBufferSize];

            //显示客户端信息
            clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("Client's endpoint is " + clientEndPoint);

            //使用NetworkStream.BeginRead异步读取信息
            NetworkStream networkStream = tcpClient.GetStream();
            networkStream.BeginRead(byteMessage, 0, tcpClient.ReceiveBufferSize,
                                         new AsyncCallback(ReceiveAsyncCallback), null);
        }

        public void ReceiveAsyncCallback(IAsyncResult iAsyncResult)
        {
            //显示CLR线程池状态
            Thread.Sleep(100);
            ThreadPoolMessage("\nMessage is receiving");


            //使用NetworkStream.EndRead结束异步读取
            int length = 0;
            try
            {
                NetworkStream networkStreamRead = tcpClient.GetStream();
                length = networkStreamRead.EndRead(iAsyncResult);
            }
            catch
            {
                Console.WriteLine("Disconnection!");
                return;
            }

            //如果接收到的数据长度少于1则抛出异常
            if (length < 1)
            {
                tcpClient.GetStream().Close();
                Console.WriteLine("Disconnection!");
                return;
            }

            //显示接收信息
            string message = "";
            if (RECEIVE_ENCODING == enuEncoding.UTF8_GetChars)
            {
                message = new string(Encoding.UTF8.GetChars(byteMessage));
            }
            else
            {
                message = Encoding.UTF8.GetString(byteMessage, 0, length);
            }
            Console.WriteLine("Message:" + message);

            //使用NetworkStream.BeginWrite异步发送信息
            byte[] sendMessage = Encoding.UTF8.GetBytes("Message is received!");
            NetworkStream networkStreamWrite = tcpClient.GetStream();
            networkStreamWrite.BeginWrite(sendMessage, 0, sendMessage.Length, new AsyncCallback(SendAsyncCallback), null);
            ////重新监听
            //tcpClient.GetStream().BeginRead(byteMessage, 0, tcpClient.ReceiveBufferSize,
            //                                   new AsyncCallback(ReceiveAsyncCallback), null);

        }

        //把信息转换成二进制数据，然后发送到客户端
        public void SendAsyncCallback(IAsyncResult iAsyncResult)
        {
            //显示CLR线程池状态
            Thread.Sleep(100);
            ThreadPoolMessage("\nMessage is sending");

            //使用NetworkStream.EndWrite结束异步发送
            tcpClient.GetStream().EndWrite(iAsyncResult);

            //重新监听
            tcpClient.GetStream().BeginRead(byteMessage, 0, tcpClient.ReceiveBufferSize,
                                               new AsyncCallback(ReceiveAsyncCallback), null);
        }

        //显示线程池现状
        static void ThreadPoolMessage(string data)
        {
            int a, b;
            ThreadPool.GetAvailableThreads(out a, out b);
            string message = string.Format("{0}\n  CurrentThreadId is {1}\n  " +
                  "WorkerThreads is:{2}  CompletionPortThreads is :{3}\n",
                  data, Thread.CurrentThread.ManagedThreadId, a.ToString(), b.ToString());

            Console.WriteLine(message);
        }
    }

}