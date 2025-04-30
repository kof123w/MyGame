using System.Diagnostics;
using System.Net;
using ConsoleApp1.TCPServer.Src.ServerParam;
using MyGame;

namespace MyServer;

//房间信息
public class Room
{
    readonly List<Player> players = new();
    public int RoomId { get; set; }
    public int RandomSeed { get; set; }

    private volatile bool IsStarted = false;
    
    private int CurRoomFrame { get; set; }
    
    private CancellationTokenSource cancellationTokenSource; 
    private List<FrameData> frameDataList = new List<FrameData>();
    
    public List<Player> Players => players;
    
    public int JoinRoom(PlayerServerData playerData)
    {
        lock (players)
        {
            for (int i = 0; i < players.Count; i++) {
                var player = players[i];
                if (player.PlayerId == playerData.RoleId)
                {
                    return i;
                }
            }

            Player newPlay = new Player();
            newPlay.PlayerId = playerData.RoleId; 
            AddPlayer(newPlay);
            return players.Count;
        }  
    }

    private void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void SetPlayerState(long playerRoleId,IPEndPoint ipEndPoint)
    {
        lock (players)
        {
            foreach (var player in players)
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
        lock (players)
        {
            bool isAllPlayerExited = true;
            foreach (var player in players)
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
            
            FramePlayerInput framePlayerInput = null;
            for (int j = 0; j < frameData.FramePlayerInputList.Count; j++) {
                if (frameData.FramePlayerInputList[j].PlayerId == sample.PlayerId)
                {
                    framePlayerInput = frameData.FramePlayerInputList[j];
                    //framePlayerInput.FrameInput.Add(sample.FrameInputList);
                    framePlayerInput.FrameInput = sample.FrameInput;
                }
            }

            if (framePlayerInput == null)
            {
                framePlayerInput = new FramePlayerInput();
                framePlayerInput.PlayerId = sample.PlayerId;
                //framePlayerInput.FrameInput.Add(sample.FrameInputList);
                framePlayerInput.FrameInput = sample.FrameInput;
                frameData.FramePlayerInputList.Add(framePlayerInput);
            }  
            
            foreach (var player in players)
            {
                if (player.PlayerId == sample.PlayerId)
                {
                    player.RunFrame = sample.ClientCurFrame;
                }
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
        players.Clear(); 
        frameDataList.Clear();
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
        //var task = Tick(cancellationTokenSource.Token); 
        var thread = new Thread(Tick) { IsBackground = true }; // 设置为后台线程
        thread.Start();
        IsStarted = true;
    } 

    private void Tick()
    {
        while (true)
        {
            try
            {
                // ReSharper disable once PossibleLossOfFraction
                PreciseDelay(1000/ LunchParam.ServerTick);
                bool isPlayerAllExited = FrameSyncLogic();
                if (isPlayerAllExited)
                {
                    CloseRoom();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private void PreciseDelay(float milliseconds)
    {
        var sw = Stopwatch.StartNew();
        var spinner = new SpinWait();
        while (sw.ElapsedMilliseconds < milliseconds)
        {
            spinner.SpinOnce();
        }
    }
    
    private bool FrameSyncLogic()
    {
        bool isAllPlayerExited = true;
        //处理服务器当前帧逻辑  
        lock (players)
        {
            foreach (var player in players)
            {
                if (player.IsPlaying)
                {
                    SCFrameData scFrameData = new SCFrameData();
                    scFrameData.FrameDataList.Add(frameDataList.Skip(player.RunFrame));
                    UDPServer.Instance.Send(MessageType.ScframeData,scFrameData,player.EndPoint);
                    isAllPlayerExited = false;
                }
            }
        }
        FrameData frameData = new FrameData();
        frameData.Frame = CurRoomFrame;
        lock (frameDataList)
        { 
            frameDataList.Add(frameData);
        }
        CurRoomFrame++;
        Console.WriteLine($"服务器，房间号:{RoomId},最新帧数{CurRoomFrame}");
        return isAllPlayerExited;
    } 

    //直接用异步的方法来处理
    private async Task Tick(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000/ LunchParam.ServerTick, token);
                FrameSyncLogic();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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