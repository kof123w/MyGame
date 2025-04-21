using System.Collections.Concurrent;
using System.Net;
using MyGame;

namespace MyServer;

//房间的逻辑
public class RoomLogic : Singleton<RoomLogic>
{
    private ConcurrentDictionary<int, Room> rooms = new ();
    
    public int JoinRoom(int roomId,IPEndPoint ipEndPoint)
    {
        if (rooms.TryGetValue(roomId,out var room))
        {
            return room.JoinRoom(ipEndPoint);
        }

        var newRoom = new Room
        {
            RoomId = roomId,
            RandomSeed = Guid.NewGuid().GetHashCode(),
        };
           
        rooms.TryAdd(roomId,newRoom);
        return newRoom.JoinRoom(ipEndPoint);
    }

    public int GetRoomRandomSeed(int roomId)
    {
        if (rooms.TryGetValue(roomId,out var room))
        {
            return room.RandomSeed;
        }
        
        return 0;
    }
}

//房间信息
public class Room
{
    List<Player> players = new List<Player>();
    public int RoomId { get; set; }
    public int RandomSeed { get; set; }

    public int JoinRoom(IPEndPoint endPoint)
    {
        lock (players)
        {
            for (int i = 0; i < players.Count; i++) {
                var player = players[i];
                if (player.EndPoint.Equals(endPoint))
                {
                    return i;
                }
            }

            Player newPlay = new Player
            {
                EndPoint = endPoint,
            };
            
            AddPlayer(newPlay);
            return players.Count;
        }  
    }

    private void AddPlayer(Player player)
    {
        players.Add(player);
    }
}

//玩家信息,临时用的
public class Player{
    public IPEndPoint EndPoint { get; set; }
}