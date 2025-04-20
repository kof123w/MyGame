using System.Reflection;
using MyServer;

namespace MyGame;

public class HandlerDispatch : Singleton<HandlerDispatch>
{
    private Dictionary<MessageType,Action<TcpServerClient,Packet>> handleActions = new (); 
    private Dictionary<Type, INetHandler> sendActions = new ();
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
                    //开启UDP的情况下就进行检测这个handler是不是要运用到udp
                    if (IsUDP)
                    {
                        object? classAttribute = type.GetCustomAttribute(typeof(UDPUseTag), false);
                        if (classAttribute is UDPUseTag udpUseTag)
                        {
                            //直接跳过
                            if (!udpUseTag.UDPUse)
                            {
                                continue;
                            }
                        } 
                    }

                    var handle = Activator.CreateInstance(type) as INetHandler; 
                    if (handle != null)
                    {
                        handle.RegNet();
                        sendActions.Add(type, handle);
                    }
                }
            }  
        }
    }

    public void RegisterHandler(MessageType type, Action<TcpServerClient, Packet> action)
    {
        handleActions.Add(type, action);
    }

    public void Dispatch(TcpServerClient serverClient, Packet packet)
    {
        handleActions[packet.Header.MessageType]?.Invoke(serverClient, packet);
    }
}