using ConsoleApp1.TCPServer.Src.Logic;
using MyServer;

namespace MyGame;

public class LoginNetHandler : INetHandler
{
    public void RegNet()
    {
        HandlerDispatch.Instance.RegisterTcpHandler(MessageType.CsloginReq,LoginHandle);
    }

    public void LoginHandle(TcpServerClient client, byte[] data)
    {
        // login handler logic  
        CSLoginReq csLoginReq = CSLoginReq.Parser.ParseFrom(data); 
        PlayerServerData pd = PlayerDataCenter.Instance.GetPlayerData(csLoginReq.UserAccount);
        pd.IsOnline = true;
        SCLoginRes scLoginRes = new SCLoginRes();
        PlayerData playerData = new PlayerData();
        playerData.PlayerRoleId = pd.RoleId;
        playerData.UserAcount = pd.Account;
        scLoginRes.PlayerData = playerData;
        client.BindPlayer(pd);
        client.Send(MessageType.ScloginRes,scLoginRes);
        
        Console.WriteLine($"收到的用户账户：{csLoginReq.UserAccount}"); 
    }
}