using System.Collections.Concurrent;
using System.Net;
using MyGame;

namespace MyServer;

//房间的逻辑
public class RoomLogic : Singleton<RoomLogic>
{
    private ConcurrentDictionary<int, Room> rooms = new ();
    
    public int JoinRoom(int roomId,PlayerServerData playerData)
    {
        if (rooms.TryGetValue(roomId,out var room))
        {
            return room.JoinRoom(playerData);
        }

        var newRoom = new Room
        {
            RoomId = roomId,
            RandomSeed = Guid.NewGuid().GetHashCode(),
        };
           
        rooms.TryAdd(roomId,newRoom);
        return newRoom.JoinRoom(playerData);
    }

    public Room GetRoom(int roomId)
    {
        if (rooms.TryGetValue(roomId, out var room))
        {
            return room;
        }

        return null;
    }

    public void SetRoomPlayerState(int roomId, long playerId,IPEndPoint ipEndPoint)
    {
        var room = GetRoom(roomId);
        if (room != null)
        {
            room.SetPlayerState(playerId, ipEndPoint);
        }
    }

    public void ExitRoom(int roomId,PlayerServerData playerData)
    {
        var room = GetRoom(roomId);
        if (room != null)
        {
            room.ExitRoom(playerData.RoleId);
            playerData.CurRoomID = 0;
        }
    }

    public void SamplePlayerHandler(CSFrameSample sample)
    {
        var room = GetRoom(sample.RoomId);
        if (room != null)
        {
            room.SampleFrame(sample);
        }
    }

    public int GetRoomRandomSeed(int roomId)
    {
        if (rooms.TryGetValue(roomId,out var room))
        {
            return room.RandomSeed;
        }
        
        return 0;
    }

    public void CloseAllRooms()
    {
        foreach (var roomPair in rooms)
        {
            var room = roomPair.Value;
            room.CloseRoom();
        }
        
        rooms.Clear();
    }
}  