using ConsoleApp1.TCPServer.Src.ServerParam;
using MyServer;

namespace MyGame;

public class MatchLogic : Singleton<MatchLogic>
{
    
    //参与匹配的玩家
    private List<PlayerData> matchedPlayers = new List<PlayerData>();

    public void JoinMatch(PlayerServerData player)
    {
        /*lock (matchedPlayers)
        {
            bool isContain = false;
            for (int i = 0; i < matchedPlayers.Count; i++)
            {
                var playerData = matchedPlayers[i];
                if (playerData.Account.Equals(player.Account))
                {
                    isContain = true;
                    break;
                }
            }

            //临时处理避免重复加入匹配列表
            if (!isContain)
            {
                matchedPlayers.Add(player);
            }
        }*/

        //临时让玩家直接进入房间内 
        int roomId = 9980; //房间号需要产生的，这里直接写死9980
        var index = RoomLogic.Instance.JoinRoom(roomId, player);
        SCMatchRes scMatchRes = new SCMatchRes();
        scMatchRes.State = 1;
        //没做中间件，临时这样处理一下写死下发UDP的地址和端口
        scMatchRes.UdpAdress = "127.0.0.1";
        scMatchRes.Port = 12900;
        scMatchRes.RoomId = roomId; //房间号需要产生的，这里直接写死9980 
        scMatchRes.PlayerIndex = index;
        scMatchRes.Tick = LunchParam.ServerTick;
        Console.WriteLine($"发送客户端匹配信息{scMatchRes.UdpAdress}");
        player.Send(MessageType.ScmatchRes, scMatchRes);
    }
}