using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputOverride : MonoBehaviour
{
    public TowerCrane tower;
    public Trolley trolley;
    public Cable cable;
    private float horizontal;
    private float vertical;
    private void Update()
    {
        RotateTower();
        MoveTrolley();
        MoveCable();
    }
    private void RotateTower()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
            tower.CraneRotation(horizontal, true);
    }
    private void MoveTrolley()
    {
        vertical = Input.GetAxis("Vertical");
        if (vertical != 0)
            trolley.TrolleyManualMove(vertical);
    }
    private void MoveCable()
    {
        float value = 0;
        if (Input.GetKey(KeyCode.LeftControl))
            value = -1f;
        if (Input.GetKey(KeyCode.Space))
            value = 1f;
        if(value != 0)
            cable.CableManualMove(value);
    }
}
