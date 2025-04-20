namespace MyServer;

[AttributeUsage(AttributeTargets.Class)]
public class UDPUseTag : Attribute
{
    public bool UDPUse { get; set; }

    public UDPUseTag(bool udpUse)
    {
        UDPUse = udpUse;
    }
}