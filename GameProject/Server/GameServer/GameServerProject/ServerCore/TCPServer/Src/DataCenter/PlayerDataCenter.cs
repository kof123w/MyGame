using System.Collections.Concurrent;
using MyServer;

namespace MyGame;

public class PlayerDataCenter : Singleton<PlayerDataCenter>
{
    private ConcurrentDictionary<long, PlayerData> playerDataCenters = new ConcurrentDictionary<long, PlayerData?>(); 

    public PlayerData GetPlayerData(string account)
    {
        foreach (var playerDataPair in playerDataCenters)
        {
            if (playerDataPair.Value.Account.Equals(account))
            {
                return playerDataPair.Value;
            } 
        }
        
        var player = GenPlayerData(account);
        playerDataCenters.TryAdd(player.RoleId, player);
        return player;
    }

    private PlayerData GenPlayerData(string account)
    {
        PlayerData playerData = new PlayerData
        {
            Account = account,
            RoleId = IDGenerator.GenHash()
        };
        return playerData;
    }
}