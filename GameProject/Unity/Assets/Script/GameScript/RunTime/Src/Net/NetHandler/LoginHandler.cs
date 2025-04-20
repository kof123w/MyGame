using Cysharp.Threading.Tasks;
using DebugTool;
using EventSystem;
using SingleTool;

namespace MyGame
{
    public class LoginHandler : INetHandler
    {
        public void RegNet()
        {
            NetManager.Instance.RegNetHandler(MessageType.ScloginRes,LoginRes);
            
            this.Subscribe<string>(NetEvent.LoginHandleEvent,LoginHandler_Login);
        }
        
        public void LoginHandler_Login(string username)
        {
            CSLoginReq csLoginReq = new CSLoginReq();
            csLoginReq.UserAccount = username; 
            NetManager.Instance.Send(MessageType.CsloginReq, csLoginReq);  
        }

        private async void LoginRes(Packet packet)
        {
            if (packet != null)
            {
                var playerData = DataCenterManger.Instance.GetDataClass<PlayerData>();
                if (playerData != null)
                {
                    var scLoginRes = ProtoHelper.Deserialize<SCLoginRes>(packet.Body.ToByteArray());
                    playerData.SetData(scLoginRes.UserAcount,scLoginRes.PlayerRoleId);
                     
                    DLogger.Log($"登录完成，账号{scLoginRes.UserAcount},进入匹配界面");
                    await UIManager.Show<MatchUI>();
                    UIManager.Close<TitleUI>();
                } 
            }
        }
    }
}