using System;
using UnityEngine;

namespace DefaultNamespace.StormFlower
{
    public class StormFlowerView : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}