using DebugTool;
using EventSystem; 

namespace MyGame
{
    public class TitleController : UIController
    {
        public override void InitController()
        { 
            this.Subscribe(UIEvent.UIEventOpenTitleUI,OpenTitleUI); 
            this.Subscribe(UIEvent.UIEventCloseTitleUI,OpenTitleUI); 
            this.Subscribe<string>(UIEvent.UIEventTitleUILogin,Login);
        }

        private async void Login(string account)
        {
             var packet = await LoginHandler.Instance.LoginHandler_Login(account);
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

        private async void OpenTitleUI()
        {  
            await UIManager.Show<TitleUI>();
        }

        private async void CloseTitleUI()
        {
            UIManager.Close<TitleUI>();
        }
    }
}