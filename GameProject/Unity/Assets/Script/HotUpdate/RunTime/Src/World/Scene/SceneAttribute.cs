using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneAttribute : Attribute
    {
        public int SceneId;
        public SceneType SceneType;
        public SceneAttribute(int sceneId, SceneType sceneType)
        {
            SceneId = sceneId;
            SceneType = sceneType;
        }
    }
}