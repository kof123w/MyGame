namespace MyServer;

// 根据游戏类型设置合适的TTL
public enum GameScope {
    LocalLan = 1,      // 本地局域网
    DataCenter = 15,   // 同园区
    CityWide = 32,     // 同城市
    Country = 64        // 全国
}