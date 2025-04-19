// See https://aka.ms/new-console-template for more information


using EventSystem;
using MyGame;
using MyServer;

Console.WriteLine("Init Event System!!!");
EventListenManager.Instance.Init();
Console.WriteLine("Init Handler Dispatch!!!");
HandlerDispatch.Instance.Init();
Console.WriteLine("Lunch Tcp Server");
TCPServer tcpServer = new TCPServer();
tcpServer.Start();



