using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private bool connected = false;
    private void OnTriggerEnter(Collider other)
    {
        if (connected)
            return;
        if(other.TryGetComponent<Concrete>(out Concrete concrete))
        {
            concrete.SetParent(this.gameObject.transform);
            connected = !connected;
        }
    }
}
