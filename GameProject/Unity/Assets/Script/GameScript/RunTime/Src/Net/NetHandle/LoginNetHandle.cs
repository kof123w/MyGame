using EventSystem;

namespace MyGame
{
    public class LoginNetHandle : INetHandle
    {
        public void RegNet()
        {
            NetManager.Instance.RegMessageType(MessageType.ScloginRes,LoginHandle);
            
            this.Subscribe<string>(DataEvent.LoginEvent,Login);
        }

        private void LoginHandle(Packet packet)
        {
            if (packet.Header.ErrorCode != 0)
            {
                return;
            }
          
            var scLoginRes = ProtoHelper.Deserialize<SCLoginRes>(packet.Body.ToByteArray());
            GameEvent.Push(DataEvent.PlayerDataEventSetData,scLoginRes.UserAcount,scLoginRes.PlayerRoleId);
            
            //启动开启
            GameEvent.Push(TaskEvent.TaskChange,typeof(LoginTask));
        }

        public void Login(string account)
        {
            CSLoginReq loginReq = new CSLoginReq();
            loginReq.UserAccount = account;
            
            NetManager.Instance.Send(MessageType.CsloginReq, ProtoHelper.CreatePacket(MessageType.CsloginReq,loginReq));
        }
    }
}