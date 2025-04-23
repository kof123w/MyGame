using Google.Protobuf; 

namespace MyGame
{
    public static class ProtoHelper
    {
        // 序列化消息
        public static byte[] Serialize(IMessage message)
        {
            using var stream = new MemoryStream();
            message.WriteTo(stream);
            return stream.ToArray();
        }
        
        // 反序列化消息
        public static T Deserialize<T>(byte[] data) where T : IMessage, new()
        {
            var message = new T();
            message.MergeFrom(data);
            return message;
        } 
    }
}