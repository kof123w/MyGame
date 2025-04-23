using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace MyServer;

public class TCPServer : Singleton<TCPServer>
{
    private TcpListener listener;
    private bool isRunning;
    private readonly ConcurrentDictionary<int, TcpServerClient> clients = new();
    private int nextClientId = 1;

    private  int Port = 12800;
    private  int MaxConnections = 100;
    private string IpAddress = "127.0.0.1";

    public void SetParam(int port, int maxConnections, string ipAddress)
    {
        Port = port;
        MaxConnections = maxConnections;
        IpAddress = ipAddress;
    }

    public void Start()
    {
        isRunning = true;
        IPAddress ipAddress = IPAddress.Parse(IpAddress);
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
            var client = new TcpServerClient(clientId, tcpClient);

            clients.TryAdd(clientId, client);

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
            if (clients.TryGetValue(clientId, out var client))
            {
                clients.Remove(clientId,out _);
                Console.WriteLine($"客户端 {clientId} 已断开连接");
            }
        }
    }
}