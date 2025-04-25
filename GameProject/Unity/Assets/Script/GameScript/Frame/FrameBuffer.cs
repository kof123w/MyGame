using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MyGame
{
    public class FrameBuffer
    {
        private readonly ConcurrentDictionary<int, FrameData> _confirmedFrames = new();
        private int _confirmedFrameId = 0;  
        private int _frameId = 1;  //默认从第一帧开始

        public int GetLastConfirmedFrameId()
        {
            return _confirmedFrameId;
        }

        public void ConfirmedFrame(int frameId)
        {
            _confirmedFrameId = frameId;
        }

        public FrameData GetNextFrame()
        {
            if (_confirmedFrames.TryGetValue(_frameId, out var frame))
            {
                _confirmedFrameId = _frameId;
                _frameId++;
                return frame;
            }

            return null;
        }

        public void AddConfirmedFrame(FrameData frame)
        {
            _confirmedFrames.TryAdd(frame.Frame, frame);
        
            // 移除旧的确认帧
            if (_confirmedFrames.Count > 100)
            {
                var toRemove = _confirmedFrames.Keys.Where(k => k < _confirmedFrameId - 50).ToList();
                foreach (var key in toRemove)
                {
                    _confirmedFrames.Remove(key, out _);
                }
            }
        }
    }
}