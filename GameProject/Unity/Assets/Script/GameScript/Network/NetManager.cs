using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SingleTool;

namespace MyGame
{
    public enum ConnectType
    {
        OffLine,  //离线
        OnLine,  //在线
    }

    public class NetManager : Singleton<NetManager>
    {
        private ConnectType connectType = ConnectType.OffLine;
        private Socket socket;
        Queue<byte[]> que = new ();

        public void SetConnectType(int connectTy)
        {
            this.connectType = (ConnectType) connectTy;
        }

        public ConnectType GetConnectType()
        {
            return connectType;
        } 
       
        public void ConnectSvr()
        {
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Connect("127.0.0.1",7000); 
            ThreadPool.QueueUserWorkItem(Receive);
        }
    
        private void Receive(object state)
        {
            byte[] buffer=new byte[1024];
            int len=socket.Receive(buffer,SocketFlags.None);
            if (len <= 0)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return;
            }
            byte[] data=buffer.Take(len).ToArray();
            //unity封住了多线程的调用，所以存入队列在主线程调用
            que.Enqueue(data);
            ThreadPool.QueueUserWorkItem(Receive); 
        }

        public void SendTxt()
        {
            if (socket.Connected)
            {
                socket.Send(Encoding.UTF8.GetBytes("你好服务器，我是客户端，我链接进来了!!!"));
            }
        }
    }
}


 