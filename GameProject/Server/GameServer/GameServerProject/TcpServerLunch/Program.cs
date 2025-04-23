// See https://aka.ms/new-console-template for more information

using EventSystem;
using MyGame;
using MyServer; 
 
class Program
{
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Task serverTcpTask;
    static Task serverUdpTask;
 
    static void Main(string[] args)
    {
        Console.WriteLine("Init Event System!!!");
        EventListenManager.Instance.Init();
        Console.WriteLine("Init Handler Dispatch!!!"); 
        HandlerDispatch.Instance.Init();
        serverTcpTask = Task.Run(RunTcpServer, cts.Token); // 启动服务器任务
        serverUdpTask = Task.Run(RunUdpServer, cts.Token);
        Thread.Sleep(1000);
        Console.WriteLine("输入exit退出"); 
        string? command = Console.ReadLine();
        while (command != null && command.Equals("exit"))
        {
            command = Console.ReadLine();
        }
        cts.Cancel();
        serverTcpTask.Wait(); 
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