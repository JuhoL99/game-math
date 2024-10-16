using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Concrete : MonoBehaviour
{
    private Transform parentObject;
    private Vector3 relativePosition;
    private Vector3 relativeAxisY;
    private Vector3 relativeAxisZ;

    void Update()
    {
        if(parentObject != null)
        {
            transform.position = parentObject.position + parentObject.TransformVector(relativePosition);
            transform.rotation = Quaternion.LookRotation(parentObject.TransformVector(relativeAxisZ), parentObject.TransformVector(relativeAxisY));
        }
    }
    public void SetParent(Transform parentT)
    {
        parentObject = parentT;
        relativePosition = parentObject.transform.InverseTransformVector(transform.position - parentObject.position);
        relativeAxisY = parentObject.transform.InverseTransformVector(transform.up);
        relativeAxisZ = parentObject.transform.InverseTransformVector(transform.forward);
    }
    public void ClearParent()
    { 
        parentObject = null; 
    }
}
