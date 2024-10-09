using NotImplementedException = System.NotImplementedException;

namespace MyGame;

public class PlayerMoveComponentSystem : ISystem,IUpdate,IStart
{
    public long EntityId { get; set; }
    public long ComponentId { get; set; }
    public long SystemId { get; set; }
    public void Update()
    {
        DLogger.Log("我触发了下 PlayerMoveComponentSystem Update");
    }

    public void Start()
    {
        DLogger.Log("我触发了下 PlayerMoveComponentSystem Start");
    }
}