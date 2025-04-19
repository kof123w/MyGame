// See https://aka.ms/new-console-template for more information

using EventSystem;
using MyGame;
using MyServer; 
using System;
using System.Threading;
using System.Threading.Tasks;
 
class Program
{
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Task serverTask;
 
    static void Main(string[] args)
    {
        serverTask = Task.Run(() => RunServer(), cts.Token); // 启动服务器任务
        Console.WriteLine("按任意键退出...");
        Console.ReadKey(); 
        cts.Cancel();
        serverTask.Wait(); 
    }
 
    static void RunServer()
    {
        Console.WriteLine("Init Event System!!!");
        EventListenManager.Instance.Init();
        Console.WriteLine("Init Handler Dispatch!!!");
        HandlerDispatch.Instance.Init();
        Console.WriteLine("Lunch Tcp Server");
        TCPServer tcpServer = new TCPServer();
        tcpServer.Start(); 
    }
}