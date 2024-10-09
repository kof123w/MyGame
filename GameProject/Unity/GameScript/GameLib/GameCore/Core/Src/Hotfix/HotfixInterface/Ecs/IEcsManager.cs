namespace MyGame;

public interface IEcsManager
{
    public ISystem GetSystem(long entityId, long componentId);

    public ISystem GetSystem(long systemId);

    public void Awake(long entityId, long componentId);

    public void OnEnable(long entityId, long componentId);

    public void Disable(long entityId, long componentId);

    public void Start(long entityId, long componentId);

    public void Destroy(long entityId, long componentId);

    public long GeneratorEntityId();

    public long GeneratorComponentId();

    public long GeneratorSystemId();
}