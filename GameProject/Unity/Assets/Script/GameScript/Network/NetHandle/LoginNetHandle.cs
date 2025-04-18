using EventSystem;

namespace MyGame
{
    public class LoginNetHandle : INetHandle
    {
        public void RegNet()
        {
            NetManager.Instance.RegMessageType(MessageType.ScloginRes,LoginHandle);
            
            this.Subscribe<string>(NetEvent.LoginEvent,Login);
        }

        private void LoginHandle(Packet packet)
        {
            ProtoHelper.Deserialize<CSLoginReq>(packet.Body.ToByteArray());
        }

        public void Login(string account)
        {
            CSLoginReq loginReq = new CSLoginReq();
            loginReq.UserAccount = account;
            
            NetManager.Instance.Send(MessageType.CsloginReq, ProtoHelper.CreatePacket(MessageType.CsloginReq,loginReq));
        }
    }
}