using DebugTool;
using EventSystem;
using Google.Protobuf;

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

        public void MatchRes(byte[] data)
        {
            SCMatchRes scMatchRes = (SCMatchRes)ProtoHelper.Deserialize<SCMatchRes>(data);
            DLogger.Log($"收到匹配回调,{scMatchRes.Relay}");
            
            UDPNetManager.Instance.Start(scMatchRes.Relay);
        }
    }
}