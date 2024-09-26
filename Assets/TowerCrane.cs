using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerCrane : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;

    public void CraneRotation(int value)
    {
        transform.Rotate(Vector3.up, value * 5);
    }
    public void CraneRotation(float value, bool keyboard)
    {
        if(keyboard)
            transform.Rotate(Vector3.up, value * rotationSpeed * Time.deltaTime);
    }
}
