using MyGame;
using MyServer;

namespace ConsoleApp1.TCPServer.Src.Logic;

public class LoginLogic : Singleton<LoginLogic>
{
    public void Login(string username)
    {
        //从数据库里面进行查询
        var playerData = PlayerDataCenter.Instance.GetPlayerData(username);
        
        //  todo 进行登录操作 ..
    }
}