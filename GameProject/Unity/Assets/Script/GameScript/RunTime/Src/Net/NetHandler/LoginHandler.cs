using Cysharp.Threading.Tasks;
using SingleTool;

namespace MyGame
{
    public class LoginHandler : Singleton<LoginHandler>
    {
        public async UniTask<Packet> LoginHandler_Login(string username)
        {
            CSLoginReq csLoginReq = new CSLoginReq();
            csLoginReq.UserAccount = username; 
            Packet packet = await NetManager.Instance.SendAsync(MessageType.CsloginReq, csLoginReq); 
            return packet;
        }
    }
}