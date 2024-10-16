using System.Collections;
using UnityEngine;

public class CraneAIControl : MonoBehaviour
{
    [SerializeField] private Transform craneForward;
    [SerializeField] private TowerCrane towerCrane;
    [SerializeField] private Trolley trolley;
    [SerializeField] private Cable cable;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform concrete;
    [SerializeField] private Transform trolleyNearLimit;
    [SerializeField] private Transform trolleyFarLimit;

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && HitConcrete())
        {
            StartCoroutine(RotateTowardConcrete());
        }
    }
    private IEnumerator RotateTowardConcrete()
    {
        while(!FacingConcrete())
        {
            towerCrane.CraneRotation(GetRotationDirection(), true);
            yield return null;
        }  
        while(!TrolleyAboveConcrete())
        {
            Debug.Log("trolley moving");
            trolley.TrolleyManualMove(GetTrolleyMoveDirection());
            yield return null;
        }
        while(!HookInConcrete())
        {
            Debug.Log("cable moving to concrete");
            cable.CableManualMove(GetCableMoveDirection());
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        while(!CableAtMaxHeight())
        {
            Debug.Log("cable moving up");
            cable.CableManualMove(1);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ResetConcrete();
        yield return null;
    }
    private void ResetConcrete()
    {
        Concrete cScript = concrete.GetComponent<Concrete>();
        Hook hScript = hook.GetComponent<Hook>();
        cScript.ClearParent();
        hScript.Disconnect();
        concrete.position = GetRandomWorldPosition();
    }
    private float GetRotationDirection()
    {
        Vector2 cranePos2D = new Vector2(towerCrane.gameObject.transform.position.x, towerCrane.gameObject.transform.position.z);
        Vector2 concretePos2D = new Vector2(concrete.position.x, concrete.position.z);
        Vector2 craneForward2D = new Vector2(craneForward.forward.x, craneForward.forward.z);
        Vector2 concreteDirection = (concretePos2D - cranePos2D).normalized;
        float signedAngle = Vector2.SignedAngle(craneForward2D, concreteDirection);
        return (int)Mathf.Sign(-signedAngle);
    }
    private bool CableAtMaxHeight()
    {
        return cable.gameObject.transform.position.y == trolley.transform.position.y;
    }
    private bool HookInConcrete()
    {
        return Mathf.Abs((hook.position.y + hook.gameObject.GetComponent<BoxCollider>().center.y) - concrete.position.y) < 0.1f; //hook attach point is different from hook origin
    }
    private int GetCableMoveDirection()
    {
        float hookY = hook.position.y + hook.gameObject.GetComponent<BoxCollider>().center.y;
        float concreteY = concrete.position.y;
        if (hookY > concreteY)
            return -1;
        return 1;
    }
    private int GetTrolleyMoveDirection()
    {
        //if distance between crane and concrete is greater than crane and trolley move pos
        Transform trolleyT = trolley.gameObject.transform;
        Vector2 concretePos2D = new Vector2(concrete.position.x, concrete.position.z);
        Vector2 trolleyPos2D = new Vector2(trolleyT.position.x, trolleyT.position.z);
        Vector2 cranePos2D = new Vector2(towerCrane.gameObject.transform.position.x, towerCrane.gameObject.transform.position.z);
        if (Vector2.Distance(cranePos2D, concretePos2D) > Vector2.Distance(cranePos2D, trolleyPos2D))
            return 1;
        return -1;

    }
    private bool TrolleyAboveConcrete()
    {
        Transform trolleyT = trolley.gameObject.transform;
        Vector2 trolleyPos2D = new Vector2(trolleyT.position.x, trolleyT.position.z);
        Vector2 concretePos2D = new Vector2(concrete.position.x, concrete.position.z);
        return Vector2.Distance(trolleyPos2D, concretePos2D) < 0.1f;
    }
    private bool FacingConcrete()
    {
        Vector2 cranePos2D = new Vector2(towerCrane.gameObject.transform.position.x, towerCrane.gameObject.transform.position.z);
        Vector2 concretePos2D = new Vector2(concrete.position.x, concrete.position.z);
        Vector2 craneForward2D = new Vector2(craneForward.forward.x, craneForward.forward.z);
        Vector2 concreteDirection = (concretePos2D - cranePos2D).normalized;
        float dot = Vector2.Dot(concreteDirection, craneForward2D);
        return dot > 0.99999f;
    }
    public Vector3 GetRandomWorldPosition()
    {
        Vector2 cranePos2D = new Vector2(towerCrane.gameObject.transform.position.x, towerCrane.gameObject.transform.position.z);
        Vector2 trolleyNearLimit2D = new Vector2(trolleyNearLimit.position.x, trolleyNearLimit.position.z);
        Vector2 trolleyFarLimit2D = new Vector2(trolleyFarLimit.position.x, trolleyFarLimit.position.z);
        float innerRadius = Vector2.Distance(cranePos2D, trolleyNearLimit2D);
        float outerRadius = Vector2.Distance(trolleyNearLimit2D, trolleyFarLimit2D);
        outerRadius += innerRadius;
        float angle = Random.Range(0, Mathf.PI * 2);
        float radius = Random.Range(innerRadius, outerRadius);

        float x = towerCrane.gameObject.transform.position.x + radius * Mathf.Cos(angle);
        float z = towerCrane.gameObject.transform.position.z + radius * Mathf.Sin(angle);
        float y = Random.Range(10f, 20f);

        return new Vector3(x,y,z);
    }
    private bool HitConcrete()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hitInfo);
        {
            return hitInfo.collider.name == "Concrete";
        }
    }

}
