using ConsoleApp1.TCPServer.Src.Logic;
using MyServer;

namespace MyGame;

public class LoginNetHandler : INetHandler
{
    public void RegNet()
    {
        HandlerDispatch.Instance.RegisterTcpHandler(MessageType.CsloginReq,LoginHandle);
    }

    public void LoginHandle(TcpServerClient client, Packet packet)
    {
        // login handler logic  
        CSLoginReq csLoginReq = CSLoginReq.Parser.ParseFrom(packet.Body); 
        PlayerData pd = PlayerDataCenter.Instance.GetPlayerData(csLoginReq.UserAccount);
        pd.IsOnline = true;
        SCLoginRes scLoginRes = new SCLoginRes();
        scLoginRes.PlayerRoleId = pd.RoleId;
        scLoginRes.UserAcount = pd.Account;
        client.BindPlayer(pd);
        client.Send(MessageType.ScloginRes,scLoginRes);
        
        Console.WriteLine($"收到的用户账户：{csLoginReq.UserAccount}"); 
    }
}