using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class Drawer : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, new Vector3(70f,1f,100f));
        }
    }
}