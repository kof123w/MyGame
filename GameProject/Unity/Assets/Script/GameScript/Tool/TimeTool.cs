using System;

namespace MyGame.Time
{
    public static class TimeTool 
    {
        /// <summary>
        /// 获取现在时间戳，秒的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampStr()
        {  
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();       
        }

        /// <summary>
        /// 获取现在时间戳，秒的长整形
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {  
            return DateTimeOffset.Now.ToUnixTimeSeconds();       
        }
        
        /// <summary>
        /// 获取现在时间戳，毫秒的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetMilliTimeStampStr()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }  

        /// <summary>
        /// 获取现在时间戳，毫秒的长整形
        /// </summary>
        /// <returns></returns>
        public static long GetMilliTimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }  

    }
}


