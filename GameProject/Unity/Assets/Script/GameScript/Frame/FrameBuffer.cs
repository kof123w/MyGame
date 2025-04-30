using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MyGame
{
    public class FrameBuffer
    {
        private readonly ConcurrentDictionary<int, FrameData> confirmedFrames = new();
        private int confirmedFrameId = 0;  
        private int frameId = 1;  //默认从第一帧开始

        public int GetLastConfirmedFrameId()
        {
            return confirmedFrameId;
        }

        public void ConfirmedFrame(int frameIdParam)
        {
            confirmedFrameId = frameIdParam;
        }

        public FrameData GetNextFrame()
        {
            if (confirmedFrames.TryGetValue(frameId, out var frame))
            {
                confirmedFrameId = frameId;
                frameId++;
                return frame;
            }

            return null;
        }

        public void AddConfirmedFrame(FrameData frame)
        {
            confirmedFrames.TryAdd(frame.Frame, frame);
        
            // 移除旧的确认帧
            if (confirmedFrames.Count > 100)
            {
                var toRemove = confirmedFrames.Keys.Where(k => k < confirmedFrameId - 50).ToList();
                foreach (var key in toRemove)
                {
                    confirmedFrames.Remove(key, out _);
                }
            }
        }
    }
}