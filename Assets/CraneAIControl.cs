using System.Collections;
using UnityEngine;

public class CraneAIControl : MonoBehaviour
{
    [SerializeField] private TowerCrane towerCrane;
    [SerializeField] private Trolley trolley;
    [SerializeField] private Cable cable;
    [SerializeField] private Transform hook;
    [SerializeField] private Transform concrete;
    [SerializeField] private Transform trolleyNearLimit;
    [SerializeField] private Transform trolleyFarLimit;

    [SerializeField] private bool debugMode = false;
    private bool craneIsMoving = false;

    private Transform craneForward;
    private Transform craneTransform;
    private Transform trolleyTransform;
    private Transform cableTransform;

    private Vector2 concretePos2D;
    private Vector2 cranePos2D;
    private Vector2 craneForward2D;
    private Vector2 trolleyPos2D;

    private Vector2 concreteDirection;

    private int gizmoCount = 1000;
    private Vector3[] gizmoPositions;


    private void Start()
    {
        craneForward = towerCrane.gameObject.transform.GetChild(0);
        craneTransform = towerCrane.gameObject.transform;
        trolleyTransform = trolley.gameObject.transform;
        cableTransform = cable.gameObject.transform;
    }
    void Update()
    {
        VectorCalculations();
        if(Input.GetMouseButtonDown(0) && HitConcrete() && !craneIsMoving)
        {
            StartCoroutine(RotateTowardConcrete());
        }
    }
    private IEnumerator RotateTowardConcrete()
    {
        float craneRotationError = 0;
        float trolleyDistanceError = 0;
        craneIsMoving = !craneIsMoving;
        while(!FacingConcrete())
        {
            if (debugMode)
                Debug.Log("crane moving");
            towerCrane.CraneRotation(GetRotationDirection(), true);
            yield return null;
        }
        craneRotationError = Mathf.Abs(Vector2.SignedAngle(craneForward2D, concreteDirection));
        while (!TrolleyAboveConcrete())
        {
            if(debugMode)
                Debug.Log("trolley moving");
            trolley.TrolleyManualMove(GetTrolleyMoveDirection());
            yield return null;
        }
        trolleyDistanceError = Vector2.Distance(trolleyPos2D, concretePos2D);
        while (!HookInConcrete())
        {
            if(debugMode)
                Debug.Log("cable moving to concrete");
            cable.CableManualMove(GetCableMoveDirection());
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        while(!CableAtMaxHeight())
        {
            if(debugMode)
                Debug.Log("cable moving up");
            cable.CableManualMove(1);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ResetConcrete();
        craneIsMoving = !craneIsMoving;
        Debug.Log($"Crane rotation error: {craneRotationError}, Trolley distance error: {trolleyDistanceError}");
        yield return null;
    }
    private void VectorCalculations()
    {
        cranePos2D = new Vector2(craneTransform.position.x, craneTransform.position.z);
        concretePos2D = new Vector2(concrete.position.x, concrete.position.z);
        craneForward2D = new Vector2(craneForward.forward.x, craneForward.forward.z);
        trolleyPos2D = new Vector2(trolleyTransform.position.x, trolleyTransform.position.z);
        concreteDirection = (concretePos2D - cranePos2D).normalized;
    }
    //disconnect concrete and set random position
    private void ResetConcrete()
    {
        Concrete cScript = concrete.GetComponent<Concrete>();
        Hook hScript = hook.GetComponent<Hook>();
        cScript.ClearParent();
        hScript.Disconnect();
        concrete.position = GetRandomWorldPosition();
    }
    //check for closest rotation direction to concrete
    private int GetRotationDirection()
    {
        float signedAngle = Vector2.SignedAngle(craneForward2D, concreteDirection);
        return (int)Mathf.Sign(-signedAngle);
    }
    //cable is at max height when its y position is same as trolley
    private bool CableAtMaxHeight()
    {
        return Mathf.Abs(cableTransform.position.y - trolleyTransform.position.y) < 0.01f;
    }
    //check if hook and concrete are close enough
    private bool HookInConcrete()
    {
        return Mathf.Abs((hook.position.y + hook.gameObject.GetComponent<BoxCollider>().center.y) - concrete.position.y) < 0.1f;
    }
    //check if concrete is above or below hook
    private int GetCableMoveDirection()
    {
        float hookY = hook.position.y + hook.gameObject.GetComponent<BoxCollider>().center.y;
        float concreteY = concrete.position.y;
        if (hookY > concreteY)
            return -1;
        return 1;
    }
    //if distance between crane and concrete is greater than crane and trolley move pos
    private int GetTrolleyMoveDirection()
    {
        if (Vector2.Distance(cranePos2D, concretePos2D) > Vector2.Distance(cranePos2D, trolleyPos2D))
            return 1;
        return -1;

    }
    //check if xz coordinates of trolley and concrete are close enough
    private bool TrolleyAboveConcrete()
    {
        return Vector2.Distance(trolleyPos2D, concretePos2D) < 0.1f;
    }
    private bool FacingConcrete()
    {
        float dot = Vector2.Dot(concreteDirection, craneForward2D);
        return dot > 0.99999f;
    }
    //get random point within a hollow cylinder around the crane
    private Vector3 GetRandomWorldPosition()
    {
        Vector2 trolleyNearLimit2D = new Vector2(trolleyNearLimit.position.x, trolleyNearLimit.position.z);
        Vector2 trolleyFarLimit2D = new Vector2(trolleyFarLimit.position.x, trolleyFarLimit.position.z);
        
        float innerRadius = Vector2.Distance(cranePos2D, trolleyNearLimit2D);
        float outerRadius = Vector2.Distance(cranePos2D, trolleyFarLimit2D);

        float angle = Random.Range(0, Mathf.PI * 2);
        float radius = Random.Range(innerRadius, outerRadius);

        float x = towerCrane.gameObject.transform.position.x + radius * Mathf.Cos(angle);
        float z = towerCrane.gameObject.transform.position.z + radius * Mathf.Sin(angle);
        float y = Random.Range(10f, 20f);

        return new Vector3(x,y,z);
    }
    //check if cursor hits the concrete
    private bool HitConcrete()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hitInfo);
        {
            return hitInfo.collider.name == "Concrete";
        }
    }
    //visualize random world point placement
    private void OnDrawGizmos()
    {
        if (!debugMode || !Application.isPlaying)
            return;
        gizmoPositions = new Vector3[gizmoCount];
        Gizmos.color = Color.red;
        for (int i = 0; i < gizmoCount; i++)
        {
            gizmoPositions[i] = GetRandomWorldPosition();
            Gizmos.DrawSphere(gizmoPositions[i], 0.5f);
        }
    }
}
