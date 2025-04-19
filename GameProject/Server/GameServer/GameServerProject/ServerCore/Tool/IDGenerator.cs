public static class IDGenerator
{ 
    private static object locker = new object();
    private static long curGenHash = 0; 
    
    //服务器临时生成ID处理，直接自增
    public static long GenHash()
    {
        lock (locker)
        {
            curGenHash++;
        }

        lock (locker)
        {
            return curGenHash;
        }
    }
}