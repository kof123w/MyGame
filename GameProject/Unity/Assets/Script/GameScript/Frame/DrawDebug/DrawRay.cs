using System;
using UnityEngine;

namespace MyGame
{
    public class DrawRay : MonoBehaviour
    {
        private Vector3 origin;
        private Vector3 direction;

        private Transform drawTrans;

        public void SetData(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction.normalized;
        }

        public void SetTrans(Transform drawTrans)
        {
            this.drawTrans = drawTrans;
        }

        private void OnDrawGizmosSelected()
        {
            if (drawTrans != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(drawTrans.position, drawTrans.forward * 10f); 
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, direction * 10f);
        }
    }
}
