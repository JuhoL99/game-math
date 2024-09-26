using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cable : MonoBehaviour
{
    [SerializeField] private Transform heightLimit;
    [SerializeField] private float cableLength = 40f;
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
        transform.position = Vector3.Lerp(heightLimit.position-new Vector3(0,cableLength,0), heightLimit.position, slider.value);
        transform.rotation = Quaternion.LookRotation(parentObject.TransformVector(relativeAxisZ), parentObject.TransformVector(relativeAxisY));
    }
    public void CableKeyboardMove(float value)
    {
        movePos += value * Time.deltaTime;
        if (movePos > 1)
            movePos = 1;
        if (movePos < 0)
            movePos = 0;
        slider.value = movePos;
    }
}
