using System.Net;
using System.Reflection;
using MyServer;

namespace MyGame;

public class HandlerDispatch : Singleton<HandlerDispatch>
{
    private Dictionary<MessageType,Action<TcpServerClient,byte[]>> tcpActions = new (); 
    private Dictionary<Type, INetHandler> sendActions = new ();
    private Dictionary<MessageType,Action<IPEndPoint, byte[]>> udpActions = new ();
    
    public bool IsUDP { get; set; }
    
    public void Init()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes(); 
        var netInterface = typeof(INetHandler);
        for (int i = 0; i < types.Length; i++)
        {
            var type = types[i];
            var interfaces = type.GetInterfaces(); 
            for (int j = 0; j < interfaces.Length; j++)
            { 
                if (interfaces[j].Name.Equals(netInterface.Name))
                {  
                    if (Activator.CreateInstance(type) is INetHandler handle)
                    {
                        handle.RegNet();
                        sendActions.Add(type, handle);
                    } 
                }
            }  
        }
    }

    public void RegisterTcpHandler(MessageType type, Action<TcpServerClient, byte[]> action)
    {
        tcpActions.Add(type, action);
    }
    
    public void RegisterUdpHandler(MessageType type, Action<IPEndPoint, byte[]> action)
    {
        udpActions.Add(type, action);
    }

    public void Dispatch(TcpServerClient serverClient, byte[] packet,MessageType type)
    {
        tcpActions[type]?.Invoke(serverClient, packet);
    }
    
    public void Dispatch(IPEndPoint clientAddress, byte[] packet,MessageType type)
    {
        udpActions[type]?.Invoke(clientAddress, packet);
    }
}