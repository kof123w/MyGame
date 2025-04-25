// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using ConsoleApp1.TCPServer.Src.Logic;
using EventSystem;
using MyGame;
using MyServer; 
 
class Program
{
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Task serverTcpTask;
    static Task serverUdpTask; 
    private static ManualResetEvent exitEvent = new ManualResetEvent(false);
    static void Main(string[] args)
    {  
        try
        {
            // 监听 Ctrl+C / 控制台关闭
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // 阻止立即退出
                Console.WriteLine("捕获 Ctrl+C，执行清理...");
                RoomLogic.Instance.CloseAllRooms();
                exitEvent.Set(); // 通知线程退出
            };

            // 监听程序退出（包括 Environment.Exit）
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.WriteLine("程序退出，执行清理...");
                RoomLogic.Instance.CloseAllRooms();
                exitEvent.Set();
            };

            Thread daemonThread = new Thread(() =>
            {
                while (!exitEvent.WaitOne(0)) // 检查是否收到退出信号
                {
                    //Console.WriteLine("守护线程运行中...");
                    Thread.Sleep(1000);
                }
                RoomLogic.Instance.CloseAllRooms();
                Console.WriteLine("守护线程安全退出！");
            });
            daemonThread.Start();
            
            Console.WriteLine("Init Event System!!!");
            EventListenManager.Instance.Init();
            Console.WriteLine("Init Handler Dispatch!!!"); 
            HandlerDispatch.Instance.Init();
            serverTcpTask = Task.Run(RunTcpServer, cts.Token); // 启动服务器任务
            serverUdpTask = Task.Run(RunUdpServer, cts.Token); 
            Console.WriteLine("主线程运行中，按 Ctrl+C 退出...");
            exitEvent.WaitOne(); // 阻塞主线程，直到收到退出信号
            cts.Cancel();
            serverTcpTask.Wait(); 
        } finally
        {
            Console.WriteLine("执行清理代码...");
            // 确保资源释放
        } 
    }

    static void RunUdpServer()
    {  
        Console.WriteLine("Lunch Tcp Server"); 
        UDPServer.Instance.SetPort(12900);
        _ = UDPServer.Instance.StartAsync(); 
    }

    static void RunTcpServer()
    {  
        Console.WriteLine("Lunch Tcp Server");
        TCPServer tcpServer = new TCPServer();
        tcpServer.SetParam(12800,100,"127.0.0.1");
        tcpServer.Start(); 
    }
}