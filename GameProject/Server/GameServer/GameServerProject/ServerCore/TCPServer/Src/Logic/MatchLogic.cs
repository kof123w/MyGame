using ConsoleApp1.TCPServer.Src.ServerParam;
using MyServer;

namespace MyGame;

public class MatchLogic : Singleton<MatchLogic>
{
    
    //参与匹配的玩家
    private List<PlayerServerData> matchedPlayers = new List<PlayerServerData>();

    public void JoinMatch(PlayerServerData player)
    {
        lock (matchedPlayers)
        {
            bool isContain = false;
            for (int i = 0; i < matchedPlayers.Count; i++)
            {
                var playerData = matchedPlayers[i];
                if (playerData.RoleId == player.RoleId)
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
        }

        //临时让玩家直接进入房间内 
        int roomId = 9980; //房间号需要产生的，这里直接写死9980
        SCMatchRes scMatchRes = new SCMatchRes();
        bool isJoinRoom = matchedPlayers.Count > 0;
        if (isJoinRoom)
        {
            RoomLogic.Instance.JoinRoom(roomId, player);
            var room = RoomLogic.Instance.GetRoom(roomId);

            for (int i = 0; i < room.Players.Count; i++) {
                var roomPlayer = room.Players[i];
                PlayerData playerData = new PlayerData
                {
                    PlayerRoleId = roomPlayer.PlayerId
                };
                scMatchRes.PlayerList.Add(playerData);
            } 
        }  
        scMatchRes.State = isJoinRoom ? 1 : 0;
        //没做中间件，临时这样处理一下写死下发UDP的地址和端口
        scMatchRes.UdpAdress = "127.0.0.1";
        scMatchRes.Port = 12900;
        scMatchRes.RoomId = roomId; //房间号需要产生的，这里直接写死9980 
        scMatchRes.RoleId = player.RoleId;
        scMatchRes.Tick = LunchParam.ServerTick; 

        Console.WriteLine($"发送客户端匹配信息{scMatchRes.UdpAdress}");
        player.Send(MessageType.ScmatchRes, scMatchRes);
    }
}