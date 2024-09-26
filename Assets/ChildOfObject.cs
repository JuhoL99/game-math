using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildOfObject : MonoBehaviour
{
    [SerializeField] private Transform parentObject;
    private Vector3 relativePosition;
    private Vector3 relativeAxisY;
    private Vector3 relativeAxisZ;
    void Start()
    {
        relativePosition = parentObject.transform.InverseTransformVector(transform.position - parentObject.position);
        relativeAxisY = parentObject.transform.InverseTransformVector(transform.up);
        relativeAxisZ = parentObject.transform.InverseTransformVector(transform.forward);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentObject.position + parentObject.TransformVector(relativePosition);
        transform.rotation = Quaternion.LookRotation(parentObject.TransformVector(relativeAxisZ), parentObject.TransformVector(relativeAxisY));
    }
}
