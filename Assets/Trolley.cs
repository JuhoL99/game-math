using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trolley : MonoBehaviour
{
    [SerializeField] private Transform farLimit;
    [SerializeField] private Transform nearLimit;
    [SerializeField] private Slider slider;

    [SerializeField] private Transform parentObject;
    private Vector3 relativeAxisY;
    private Vector3 relativeAxisZ;

    private float movePos = 1;
    void Start()
    {
        relativeAxisY = parentObject.transform.InverseTransformVector(transform.up);
        relativeAxisZ = parentObject.transform.InverseTransformVector(transform.forward);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(nearLimit.position, farLimit.position, slider.value);
        transform.rotation = Quaternion.LookRotation(parentObject.TransformVector(relativeAxisZ), parentObject.TransformVector(relativeAxisY));
    }
    public void TrolleyKeyboardMove(float value)
    {
        movePos += value * Time.deltaTime;
        if (movePos > 1)
            movePos = 1;
        if (movePos < 0)
            movePos = 0;
        slider.value = movePos;
    }
}
