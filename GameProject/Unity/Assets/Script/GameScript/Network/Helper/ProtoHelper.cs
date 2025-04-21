using System.IO;
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
        
        public static T Deserialize<T>(Packet packet) where T : IMessage, new()
        {
            var message = new T();
            message.MergeFrom(packet.Body.ToByteArray());
            return message;
        }

        
        // 创建数据包
        public static Packet CreatePacket(MessageType messageType, IMessage body, uint errorCode = 0)
        {
            return new Packet
            {
                Header = new MessageHeader()
                {
                    MessageType = messageType,
                    ErrorCode = errorCode
                }, 
                Body = body.ToByteString()
            };
        }
        
        public static Packet CreatePacket(MessageType messageType, CSLoginReq body, uint errorCode = 0)
        {
            return new Packet
            {
                Header = new MessageHeader()
                {
                    MessageType = messageType,
                    ErrorCode = errorCode
                }, 
                Body = body.ToByteString()
            };
        }
    }
}