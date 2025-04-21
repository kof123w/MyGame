using System.Net.Sockets;
using EventSystem;
using MyGame;
using MyServer;

class Program
{
    static CancellationTokenSource cts = new CancellationTokenSource();
    static Task serverTask;
 
    static void Main(string[] args)
    {
        serverTask = Task.Run(() => RunServer(), cts.Token); // 启动服务器任务
        Console.WriteLine("输入exit退出"); 
        string? command = Console.ReadLine();
        while (command != null && command.Equals("exit"))
        {
            command = Console.ReadLine();
        }

        cts.Cancel();
        serverTask.Wait(); 
    }
 
    static void RunServer()
    {
        Console.WriteLine("Init Event System!!!");
        EventListenManager.Instance.Init();
        Console.WriteLine("Init Handler Dispatch!!!");
        HandlerDispatch.Instance.IsUDP = true;
        HandlerDispatch.Instance.Init();
        Console.WriteLine("Lunch Tcp Server"); 
        UDPServer.Instance.SetPort(12900);
        _ = UDPServer.Instance.StartAsync(); 
    }
}