using System.Reflection;
using MyServer;

namespace MyGame;

public class HandlerDispatch : Singleton<HandlerDispatch>
{
    private Dictionary<MessageType,Action<TcpServerClient,Packet>> handleActions = new (); 
    
    private Dictionary<Type, INetHandler> sendActions = new ();
    
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