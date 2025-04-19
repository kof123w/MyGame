using MyServer;

namespace MyGame;

public class LoginNetHandler : INetHandler
{
    public void RegNet()
    {
        HandlerDispatch.Instance.RegisterHandler(MessageType.CsloginReq,LoginHandle);
    }

    public void LoginHandle(Client client, Packet packet)
    {
        // login handler logic  
        CSLoginReq csLoginReq = CSLoginReq.Parser.ParseFrom(packet.Body); 
        SCLoginRes scLoginRes = new SCLoginRes();
        scLoginRes.PlayerRoleId = IDGenerator.GenHash();
        scLoginRes.UserAcount = csLoginReq.UserAccount;
        
        client.Send(MessageType.ScloginRes,scLoginRes);
        
        Console.WriteLine($"收到的用户账户：{csLoginReq.UserAccount}"); 
    }
}