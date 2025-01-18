namespace MyGame
{
    public interface ISystem
    {
        public long EntityId { get; set; }
        public long ComponentId { get; set; }
        public long SystemId { get; set; }
    }
}