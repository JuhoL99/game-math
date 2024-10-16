using System.Collections;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GetBoxLocation();
        }
    }
    private void GetBoxLocation()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hitInfo);
        {
            if (hitInfo.collider.name == "Concrete")
            {
                Vector3 location = hitInfo.transform.position;
            }
        }
    }
}
