using System.Collections.Concurrent;
using MyServer;

namespace MyGame;

public class PlayerDataCenter : Singleton<PlayerDataCenter>
{
    private readonly ConcurrentDictionary<long, PlayerServerData> _playerDataCenters = new(); 

    public PlayerServerData GetPlayerData(string account)
    {
        foreach (var playerDataPair in _playerDataCenters)
        {
            if (playerDataPair.Value.Account.Equals(account))
            {
                return playerDataPair.Value;
            } 
        }
        
        var player = GenPlayerData(account);
        _playerDataCenters.TryAdd(player.RoleId, player);
        return player;
    }

    private PlayerServerData GenPlayerData(string account)
    {
        PlayerServerData playerData = new PlayerServerData
        {
            Account = account,
            RoleId = IDGenerator.GenHash()
        };
        return playerData;
    }
}