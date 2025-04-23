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
            SCMatchRes scMatchRes = ProtoHelper.Deserialize<SCMatchRes>(data);
            DLogger.Log($"收到匹配回调,{scMatchRes.UdpAdress}:{scMatchRes.Port}");
            UDPNetManager.Instance.Start(scMatchRes.UdpAdress,scMatchRes.Port,scMatchRes.RoomId);
            
            var pd = DataCenterManger.Instance.GetDataClass<PlayerData>();
            CSPostClientUdpAddress csPostClientUdpAddress = new CSPostClientUdpAddress();
            csPostClientUdpAddress.RoomId = scMatchRes.RoomId;
            csPostClientUdpAddress.PlayerId = pd.GetId(); 
            UDPNetManager.Instance.Send(MessageType.CspostClientUdpAddress,csPostClientUdpAddress);
            FrameLogic.Instance.Start(scMatchRes.RandomSeed,scMatchRes.Tick,UDPNetManager.Instance);
        }
    }
}