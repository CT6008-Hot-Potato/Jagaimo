using System;
using UnityEngine;

public class JagaimoPlayerSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        JagaimoNetworkManager.AddSpawnPoint(base.transform);
    }
    
    private void OnDestroy()
    {
        JagaimoNetworkManager.RemoveSpawnPoint(base.transform);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}