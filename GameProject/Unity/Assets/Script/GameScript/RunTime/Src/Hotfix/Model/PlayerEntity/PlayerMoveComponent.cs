using UnityEngine;

namespace MyGame
{
    [System(typeof(PlayerMoveComponentSystem))]
    public class PlayerMoveComponent : ComponentData
    {
        public float Speed = 0.0f;
        public Vector3 Pos = Vector3.zero;
    }
}

