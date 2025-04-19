using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace MyServer;

public class TCPServer : Singleton<TCPServer>
{
    private TcpListener listener;
    private bool isRunning;
    private readonly Dictionary<int, TCPClient> clients = new();
    private int nextClientId = 1;

    private const int Port = 12800;
    private const int MaxConnections = 100;

    public void Start()
    {
        isRunning = true;
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        listener = new TcpListener(ipAddress, Port);
        listener.Start(MaxConnections);

        Console.WriteLine($"游戏服务器已启动，监听端口 {Port}...");
        listener.BeginAcceptTcpClient(OnClientConnected, null);
    }

    public void Stop()
    {
        isRunning = false;
        listener.Stop();

        lock (clients)
        {
            foreach (var client in clients.Values)
            {
                client.Disconnect();
            }

            clients.Clear();
        }

        Console.WriteLine("游戏服务器已停止");
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        if (!isRunning) return;

        try
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(OnClientConnected, null);

            int clientId = nextClientId++;
            var client = new TCPClient(clientId, tcpClient);

            lock (clients)
            {
                clients.Add(clientId, client);
            }

            Console.WriteLine($"客户端 {clientId} 已连接，来自 {tcpClient.Client.RemoteEndPoint}");
            client.BeginReceive();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"接受客户端连接时出错: {ex.Message},{ex.StackTrace}");
        }
    }

    public void OnClientDisconnected(int clientId)
    {
        lock (clients)
        {
            if (clients.TryGetValue(clientId, out TCPClient client))
            {
                clients.Remove(clientId);
                Console.WriteLine($"客户端 {clientId} 已断开连接");
            }
        }
    }
}