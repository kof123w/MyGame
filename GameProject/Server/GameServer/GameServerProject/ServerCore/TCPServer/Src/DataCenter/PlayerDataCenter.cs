using System.Collections.Concurrent;
using MyServer;

namespace MyGame;

public class PlayerDataCenter : Singleton<PlayerDataCenter>
{
    private ConcurrentDictionary<long,PlayerDataCenter> playerDataCenters = new ConcurrentDictionary<long, PlayerDataCenter>();

    public PlayerData GenPlayerData(string account)
    {
        PlayerData playerData = new PlayerData
        {
            Account = account,
            RoleId = IDGenerator.GenHash()
        };
        return playerData;
    }
}