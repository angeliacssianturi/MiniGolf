using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class BallController : MonoBehaviour
{
    public static BallController instance;

    [SerializeField] private LineRenderer LineRenderer;
    [SerializeField] private GameObject areaAffector;
    [SerializeField] private float maxForce, forceModifier;
    [SerializeField] private LayerMask rayLayer;

    private float force;
    private Rigidbody rgBody;

    private Vector3 startPos, endPos;
    private bool canShoot = false;
    private Vector3 direction;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        rgBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        CameraFollow.instance.SetTarget(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !canShoot)
        {
            startPos = ClickedPoint();
            LineRenderer.gameObject.SetActive(true);
            LineRenderer.SetPosition(0, LineRenderer.transform.localPosition);
        }

        if (Input.GetMouseButton(0) && canShoot)
        {
            endPos = ClickedPoint();
            endPos.y = LineRenderer.transform.position.y;
            force = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, maxForce);
            LineRenderer.SetPosition(1, transform.InverseTransformPoint(endPos));
        }

        if (Input.GetMouseButtonUp(0) && canShoot)
        {
            rgBody.AddForce(direction.normalized * force, ForceMode.Impulse);
            canShoot = false;
            LineRenderer.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if(canShoot)
        {
            canShoot = false;
            direction = startPos - endPos;
            rgBody.AddForce(direction * force, ForceMode.Impulse);
            force = 0;
            startPos = endPos = Vector3.zero;
        }
    }

    Vector3 ClickedPoint()
    {
        Vector3 position = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, rayLayer))
        {
            position = hit.point;
        }
        return position;
    }
}
