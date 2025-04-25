using System;
using System.Buffers;
using System.Collections.Generic;
using Google.Protobuf;
using MyGame;
using SingleTool;
using UnityEngine;

public class UDPNetManager : Singleton<UDPNetManager>,INetworkService
{
    private UdpLocalClient udpLocalClient;
    private int curRoomId; 
    public event Action<int,byte[]> OnReceived ;
    private Dictionary<MessageType, Action<byte[]>> Dispatch = new();
    private List<INetHandler> NetHandlers = new();
    public void Start(string udpAddress, int port,int roomId)
    { 
           
        curRoomId = roomId;
        udpLocalClient = new UdpLocalClient (udpAddress, port);
        udpLocalClient.StartReceive();
        
        // todo .. 创建出来临时直接加入发加入房间
        //CSJoinRoom csJoinRoom = new CSJoinRoom();
        //csJoinRoom.RoomId = roomId;
        //Send(MessageType.CsjoinRoom, csJoinRoom);  
    }

    public void Send<T>(MessageType type, T t) where T : IMessage,new()
    { 
        Send((int)type,ProtoHelper.Serialize(t));
    }

    public void Send(int type,byte[] data)
    {
        udpLocalClient.Send(type,data);
    }
    
    public void AddHandler(INetHandler handler)
    {
        handler?.RegNet();
        NetHandlers.Add(handler);
    }

    public void RegNetHandler(MessageType type, Action<byte[]> handler)
    {
        Dispatch.Add(type, handler);
    }

    public void HandlerDispatch(byte[] data)
    { 
        var prefixBuffer = ArrayPool<byte>.Shared.Rent(4);
        Array.Copy(data, 0, prefixBuffer, 0, 4); 
        MessageType type = (MessageType)BitConverter.ToInt32(prefixBuffer, 0);
        
        if (Dispatch.TryGetValue(type, out var handlerAction))
        { 
            handlerAction?.Invoke(data.AsSpan(4,data.Length - 4).ToArray());
        }

        ArrayPool<byte>.Shared.Return(prefixBuffer);
    }

    public void Disconnect()
    {
        udpLocalClient?.Close();
    }
}
