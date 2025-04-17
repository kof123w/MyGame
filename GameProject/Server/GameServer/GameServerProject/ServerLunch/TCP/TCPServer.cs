using System.Net;
using System.Net.Sockets;

namespace MyServer;

public class TCPServer : Singleton<TCPServer>
{
    private TcpListener listener;
    private bool isRunning;
    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private int nextClientId = 1;

    // 服务器配置
    private const int Port = 5555;
    private const int MaxConnections = 100;

    public void Start()
    {
        isRunning = true;
        listener = new TcpListener(IPAddress.Any, Port);
        listener.Start(MaxConnections);

        Console.WriteLine($"游戏服务器已启动，监听端口 {Port}...");

        // 开始接受客户端连接
        listener.BeginAcceptTcpClient(OnClientConnected, null);
    }

    public void Stop()
    {
        isRunning = false;
        listener.Stop();

        // 断开所有客户端
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

            // 继续监听新连接
            listener.BeginAcceptTcpClient(OnClientConnected, null);

            // 处理新客户端
            int clientId = nextClientId++;
            var client = new Client(clientId, tcpClient);

            lock (clients)
            {
                clients.Add(clientId, client);
            }

            Console.WriteLine($"客户端 {clientId} 已连接，来自 {tcpClient.Client.RemoteEndPoint}");

            // 开始接收客户端数据
            client.BeginReceive();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"接受客户端连接时出错: {ex.Message}");
        }
    }

    public void OnClientDisconnected(int clientId)
    {
        lock (clients)
        {
            if (clients.TryGetValue(clientId, out Client client))
            {
                clients.Remove(clientId);
                Console.WriteLine($"客户端 {clientId} 已断开连接");
            }
        }
    }
}