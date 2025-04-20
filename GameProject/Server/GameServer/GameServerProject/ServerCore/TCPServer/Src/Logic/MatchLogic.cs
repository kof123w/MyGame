using MyServer;

namespace MyGame;

public class MatchLogic : Singleton<MatchLogic>
{
    
    //参与匹配的玩家
    private List<PlayerData> matchedPlayers = new List<PlayerData>();
    
    public void JoinMatch(PlayerData player)
    {
        lock (matchedPlayers)
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
        }
        
        SCMatchRes scMatchRes = new SCMatchRes();
        scMatchRes.State = 1;
        //没做中间件，临时这样处理一下写死下发UDP的地址和端口
        scMatchRes.Relay = "127.0.0.1&12900&9981";
        Console.WriteLine($"发送客户端匹配信息{scMatchRes.Relay}");
        player.Send(MessageType.ScmatchRes, scMatchRes);
    }
}