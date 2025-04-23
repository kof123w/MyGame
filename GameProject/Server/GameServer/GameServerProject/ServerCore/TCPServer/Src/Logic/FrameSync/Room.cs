using System.Net;
using ConsoleApp1.TCPServer.Src.ServerParam;
using MyGame;

namespace MyServer;

//房间信息
public class Room
{
    readonly List<Player> _players = new();
    public int RoomId { get; set; }
    public int RandomSeed { get; set; }

    public volatile bool IsStarted = false;
    
    public int CurRoomFrame { get; set; }
    
    private CancellationTokenSource cancellationTokenSource; 
    private List<FrameData> frameDataList = new List<FrameData>();
    
    public int JoinRoom(PlayerServerData playerData)
    {
        lock (_players)
        {
            for (int i = 0; i < _players.Count; i++) {
                var player = _players[i];
                if (player.PlayerId == playerData.RoleId)
                {
                    return i;
                }
            }

            Player newPlay = new Player();
            newPlay.PlayerId = playerData.RoleId; 
            AddPlayer(newPlay);
            return _players.Count;
        }  
    }

    private void AddPlayer(Player player)
    {
        _players.Add(player);
    }

    public void SetPlayerState(long playerRoleId,IPEndPoint ipEndPoint)
    {
        lock (_players)
        {
            foreach (var player in _players)
            {
                if (player.PlayerId == playerRoleId)
                {
                    player.PlayerId = playerRoleId;
                    player.IsPlaying = true;
                    player.EndPoint = ipEndPoint;
                }
            }
        }

        if (!IsStarted)
        {
            StartRoom();
        }
    }

    public void ExitRoom(long playerRoleId)
    {
        lock (_players)
        {
            bool isAllPlayerExited = true;
            foreach (var player in _players)
            {
                if (player.PlayerId == playerRoleId)
                { 
                    player.IsPlaying = false; 
                }

                if (player.IsPlaying)
                {
                    isAllPlayerExited = false;
                } 
            }

            if (isAllPlayerExited)
            {
                IsStarted = false;
                CloseRoom();
            }
        }
    }

    public void SampleFrame(CSFrameSample sample)
    {
        lock (frameDataList)
        {
            if (frameDataList.Count == 0)
            {
                return;
            }
            var frameData = frameDataList[^1];
            for (int i = 0; i < sample.FrameInputList.Count; i++)
            {
                FramePlayerInput framePlayerInput = new FramePlayerInput();
                framePlayerInput.FrameInput = sample.FrameInputList[i];
                framePlayerInput.PlayerId = sample.PlayerId;
                frameData.FramePlayerInputList.Add(framePlayerInput);
            } 
        }

      
    }

    public void CloseRoom()
    {
        if (cancellationTokenSource?.IsCancellationRequested == false)
        { 
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose(); 
        }
        _players.Clear(); 
    }

    public void StartRoom()
    {
        CurRoomFrame = 1;
        if (cancellationTokenSource?.IsCancellationRequested == false)
        { 
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose(); 
        }
        cancellationTokenSource = new CancellationTokenSource();
        Tick(cancellationTokenSource.Token);
    }
    
    //直接用异步的方法来处理
    private async Task Tick(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(1000/LunchParam.ServerTick, token);
            //处理服务器当前帧逻辑
            FrameData frameData = new FrameData();
            frameData.Frame = CurRoomFrame + 1;
            lock (frameDataList)
            { 
                frameDataList.Add(frameData);
            }

            lock (_players)
            {
                foreach (var player in _players)
                {
                    if (player.IsPlaying)
                    {
                        SCFrameData scFrameData = new SCFrameData();
                        scFrameData.FrameDataList.Add(frameDataList.Skip(player.RunFrame));
                        UDPServer.Instance.Send(MessageType.ScframeData,scFrameData,player.EndPoint);
                    }
                }
            }
            
            CurRoomFrame++;
        }
    }
}

//玩家信息,临时用的
public class Player{
    public long PlayerId { get; set; }
    public IPEndPoint EndPoint { get; set; }
    public int RunFrame { get; set; }
    public bool IsPlaying { get; set; }
}