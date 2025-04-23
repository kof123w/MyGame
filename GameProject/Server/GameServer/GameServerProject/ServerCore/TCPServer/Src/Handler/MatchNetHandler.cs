using MyServer;

namespace MyGame;

public class MatchNetHandler : INetHandler
{
    public void RegNet()
    {
        HandlerDispatch.Instance.RegisterTcpHandler(MessageType.CsmatchReq,MatchHandle);
    }

    private void MatchHandle(TcpServerClient client,byte[] packet)
    {
         MatchLogic.Instance.JoinMatch(client.GetPlayer());
    }
}