using DebugTool;
using EventSystem;

namespace MyGame
{
    public class MatchHandler : INetHandler
    {
        public void RegNet()
        {
            NetManager.Instance.RegNetHandler(MessageType.ScmatchRes,MatchRes);
            
            this.Subscribe(NetEvent.MatchHandleEvent,JoinMatch);
        }

        private void JoinMatch()
        {
            CSMatchReq csMatchReq = new CSMatchReq();
            NetManager.Instance.Send(MessageType.CsmatchReq, csMatchReq); 
        }

        public void MatchRes(Packet packet)
        {
            SCMatchRes scMatchRes = ProtoHelper.Deserialize<SCMatchRes>(packet);
            DLogger.Log($"收到匹配回调,{scMatchRes.Relay}");
            
            UDPServer.Instance.Start(scMatchRes.Relay);
        }
    }
}