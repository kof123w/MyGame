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
using SingleTool;
using UnityEngine;

namespace MyGame
{
    public class NetManager : Singleton<NetManager>
    {
        private Dictionary<MessageType,Action<Packet>> handleActions = new Dictionary<MessageType, Action<Packet>>(); 
        private ConcurrentQueue<Packet> tcpPacketQueue = new ConcurrentQueue<Packet>(); 
        private List<INetHandle> handles = new List<INetHandle>();
        
        private TcpLocalClient tcpClient;
        
        private string serverIP = "127.0.0.1";
        private int serverPort = 12800;
        
        public bool IsConnected {
            get
            {
                if (tcpClient == null)
                {
                    return false;
                }
                
                return tcpClient.IsConnected;
            }
        }
        
        //等待的回报类型
        private volatile MessageType waitMessageType = MessageType.None;
        
        public void InitTcpHandle()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes(); 
            var netInterface = typeof(INetHandle);
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var interfaces = type.GetInterfaces(); 
                for (int j = 0; j < interfaces.Length; j++)
                { 
                    if (interfaces[j].Name.Equals(netInterface.Name))
                    {
                        var handle = Activator.CreateInstance(type) as INetHandle;
                        handle?.RegNet();
                        handles.Add(handle);
                    }
                }  
            }
            
            //开启线程处理分发协议
            Task.Run(PacketDispatch);
        }

        public async UniTask ConnectServer()
        {
            tcpClient = new TcpLocalClient();
            await tcpClient.Connected(serverIP, serverPort);
        }

        public void RegMessageType(MessageType type,Action<Packet> action)
        {
            handleActions.Add(type,action);
        }

        private void PacketDispatch()
        {
            while (true)
            {
                tcpPacketQueue.TryDequeue(out var packet);
                if(packet!=null){
                    if (waitMessageType == packet.Header.MessageType - 1000000)
                    {
                        waitMessageType = MessageType.None;
                    }

                    if (handleActions.ContainsKey(packet.Header.MessageType))
                    {
                        handleActions[packet.Header.MessageType]?.Invoke(packet);
                    }
                }
            }
        }

        public void AddPacket(Packet packet)
        {
            tcpPacketQueue.Enqueue(packet);
        }

        private CancellationTokenSource tokenSource;

        public async void Send(MessageType type, Packet packet,bool wait = true)
        {
            try
            {
                if (!IsConnected)
                {
                    await ConnectServer();
                }

                if (!wait)
                {
                    tcpClient.Send(type, packet);
                }
                else
                {
                    AwaitSend(type, packet);
                }
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        private async UniTaskVoid AwaitSend(MessageType type, Packet packet)
        {
            waitMessageType = type;
            tcpClient.Send(type, packet);
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            var task = AwaitRev(tokenSource.Token);
            var timeout3S = UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token).Token);
            var (_,index1) = await UniTask.WhenAny(task, timeout3S).SuppressCancellationThrow();
            if (index1 == 1)
            { 
                GameEvent.Push(NetEvent.OpenWaitNetUI);
                task = AwaitRev(tokenSource.Token);
                var timeout6S = UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken:  CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token).Token);
                var (_,index2) = await UniTask.WhenAny(task, timeout6S).SuppressCancellationThrow();
                
                if (index2 == 1)
                {
                    DLogger.Log("协议返回超时");
                    tokenSource.Cancel();
                    GameEvent.Push(NetEvent.CloseWaitNetUI);
                }
            }
            
            if (task.Status.IsCompletedSuccessfully())
            {
                DLogger.Log("任务正常完成");
            }
        }

        private async UniTask AwaitRev(CancellationToken token)
        {
            await UniTask.WaitUntil(()=>waitMessageType == MessageType.None,cancellationToken: token);
        }
    }
}