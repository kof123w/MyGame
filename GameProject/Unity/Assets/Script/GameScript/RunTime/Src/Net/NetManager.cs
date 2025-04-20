using System;
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
        private AsyncTcpClient asyncTcpClient; 
        private string serverIP = "127.0.0.1";
        private int serverPort = 12800;
        
        private Dictionary<MessageType, Action<Packet>> Dispatch = new();
        private ConcurrentQueue<Packet> PacketQueue = new();
        private List<INetHandler> NetHandlers = new();
        private CancellationTokenSource checkPacketCts;
        //等待的回报类型
        private volatile MessageType waitMessageType = MessageType.None;
        
        private bool IsConnected {
            get
            {
                if (asyncTcpClient == null)
                {
                    return false;
                }
                
                return asyncTcpClient.IsConnected;
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

        public void AddPacket(Packet packet)
        {
            PacketQueue.Enqueue(packet);
        }

        public void RegNetHandler(MessageType messageType, Action<Packet> handler)
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
                    if (packet != null && packet.Header!=null)
                    {
                        if (packet.Header.MessageType == waitMessageType + 1000000)
                        {
                            waitMessageType = MessageType.None;
                        }

                        if (Dispatch.TryGetValue(packet.Header.MessageType, out var handlerAction))
                        {
                            handlerAction(packet);
                        }
                    } 
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
                GameEvent.Push(UIEvent.OpenWaitNetUI);
            }

            if (waitSendCount >= 6)
            {
                if (sendTokenSource?.IsCancellationRequested == false)
                {
                    sendTokenSource?.Cancel();
                    sendTokenSource?.Dispose();
                }  
                GameEvent.Push(UIEvent.CloseWaitNetUI);
                waitSendCount = 0;
                waitMessageType = MessageType.None;
            }
        }

        private async UniTask<bool> ConnectServer()
        {
            if (asyncTcpClient!=null)
            {
                asyncTcpClient.Disconnect();
                asyncTcpClient = null;
                await UniTask.Delay(100); 
            }

            asyncTcpClient = new AsyncTcpClient();
            return await asyncTcpClient.Connected(serverIP, serverPort);
        } 

        private CancellationTokenSource sendTokenSource; 

        public async UniTask<Packet> SendAsync<T>(MessageType type,T t) where T : IMessage,new()
        {
            if (waitMessageType != MessageType.None)
            {
                return null;
            }

            if (!IsConnected)
            {
                bool isCom = await ConnectServer();
                if (!isCom)
                {
                    return null;
                }
            }

            if (sendTokenSource?.IsCancellationRequested == false)
            {
                sendTokenSource?.Cancel(); 
            } 
            sendTokenSource?.Dispose();
            sendTokenSource = new CancellationTokenSource(); 
            waitMessageType = type;
            Packet packet = ProtoHelper.CreatePacket(type,t);
            var packetTask = await asyncTcpClient.SendAsync(packet,sendTokenSource.Token);     
            waitMessageType = MessageType.None;
            return packetTask;
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
            Packet packet = ProtoHelper.CreatePacket(type,t);
            asyncTcpClient.Send(packet);      
        }   
    }
}