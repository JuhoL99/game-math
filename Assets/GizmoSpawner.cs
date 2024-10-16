using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSpawner : MonoBehaviour
{
    //check if random concrete spawns are correct
    [SerializeField] private CraneAIControl craneC;
    private int gizmoCount = 1000;
    private Vector3[] gizmoPositions;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        gizmoPositions = new Vector3[gizmoCount];
        for (int i = 0; i < gizmoCount; i++)
        {
            gizmoPositions[i] = craneC.GetRandomWorldPosition();
        }
        Gizmos.color = Color.red;
        foreach (Vector3 pos in gizmoPositions)
        {
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}
