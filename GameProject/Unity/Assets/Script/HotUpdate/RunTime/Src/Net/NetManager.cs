using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DebugTool;
using EventSystem;
using GameTimer;
using Google.Protobuf;
using SingleTool;
using UnityEngine;

namespace MyGame
{
    public class NetManager : Singleton<NetManager>
    { 
        private TcpLocalClient tcpLocalClient; 
        private string serverIP = "127.0.0.1";
        private int serverPort = 12800; 
        private Dictionary<MessageType, Action<byte[]>> Dispatch = new();
        private ConcurrentQueue<byte[]> PacketQueue = new();
        private List<INetHandler> NetHandlers = new();
        private CancellationTokenSource checkPacketCts;
        
        //等待的回报类型
        private volatile MessageType waitMessageType = MessageType.None;
        
        private bool IsConnected {
            get
            {
                if (tcpLocalClient == null)
                {
                    return false;
                }
                
                return tcpLocalClient.IsConnected;
            }
        }
        
        public void Init()
        {
            DLogger.Log("==============>Init NetManager");
            GameTimerManager.CreateLoopSecondTimer("NetManager_CheckTimeOut",1.0f,CheckTimeout);
            checkPacketCts = new CancellationTokenSource();
            CheckPacket(checkPacketCts.Token);
        }

        public void AddHandler(INetHandler handler)
        {
            handler?.RegNet();
            NetHandlers.Add(handler);
        }

        public void AddPacket(byte[] packet)
        {
            PacketQueue.Enqueue(packet);
        }

        public void RegNetHandler(MessageType messageType, Action<byte[]> handler)
        {
            Dispatch.Add(messageType, handler);
        }

        private async void CheckPacket(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield();
                if (PacketQueue.TryDequeue(out var packet))
                {
                    var prxBuffer = ArrayPool<byte>.Shared.Rent(4);
                    Array.Copy(packet, 0, prxBuffer, 0, 4);
                    
                    MessageType messageType = (MessageType)BitConverter.ToInt32(prxBuffer, 0);
                    if (messageType == waitMessageType + 1000000)
                    {
                        waitMessageType = MessageType.None;
                    }

                    if (Dispatch.TryGetValue(messageType, out var handlerAction))
                    {
                        var data = new byte[packet.Length - 4];
                        Array.Copy(packet, 4, data, 0, packet.Length - 4);
                        handlerAction(data.AsSpan(0,packet.Length - 4).ToArray());
                    }
                    
                    ArrayPool<byte>.Shared.Return(prxBuffer);
                }
            }
        }

        private int waitSendCount = 0; 
        private void CheckTimeout()
        {
            if (waitMessageType != MessageType.None)
            {
                waitSendCount += 1;
            }
            else
            {
                waitSendCount = 0;
            }

            if (waitSendCount == 3)
            {
                GameEvent.Push(WaitNetUIEvent.OpenWaitNetUI);
            }

            if (waitSendCount >= 6)
            { 
                GameEvent.Push(WaitNetUIEvent.CloseWaitNetUI);
                waitSendCount = 0;
                waitMessageType = MessageType.None;
            }
        }

        private async UniTask<bool> ConnectServer()
        {
            if (tcpLocalClient!=null)
            {
                tcpLocalClient.Disconnect();
                tcpLocalClient = null;
                await UniTask.Delay(100); 
            }

            tcpLocalClient = new TcpLocalClient();
            return await tcpLocalClient. Connected(serverIP, serverPort);
        }  
        
        
        public async void Send<T>(MessageType type,T t) where T : IMessage,new()
        {
            if (waitMessageType != MessageType.None)
            {
                return;
            }

            if (!IsConnected)
            {
                bool isCom = await ConnectServer();
                if (!isCom)
                {
                    return;
                }
            } 
            waitMessageType = type; 
            tcpLocalClient.Send(type,t);      
        }

        public void Disconnect()
        {
            tcpLocalClient?.Disconnect();
        }
    }
}