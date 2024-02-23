using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooter : MonoBehaviour
{
    public float maxDistance;

    public Rope ropeSettings;
    [HideInInspector]
    public Rope rope;
    public Vector3 grapplePoint;
    [SerializeField]
    private GameObject projectilePrefab;
    [HideInInspector]
    private GameObject projectileInst;
    public Rigidbody rb;
    public LineRenderer lineRenderer;
    public LayerMask swingableSurfaces;
    public int mouseButton;

    public bool ropeOut = false;

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
    }

    void ShootRope()
    {
        ropeOut = true;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2));
        if (Physics.Raycast(ray, out hit, maxDistance, swingableSurfaces))
        {
            grapplePoint = hit.point;
            if (projectileInst == null) projectileInst = GameObject.Instantiate(projectilePrefab);
            projectileInst.transform.position = grapplePoint;

            rope = new Rope(projectileInst.transform.position, transform.position, ropeSettings);
            rope.Attach(projectileInst, rope.Endpoint1);
            rope.Attach(rb.gameObject, rope.Endpoint2);

            float distance = Vector3.Distance(grapplePoint, rb.position);

        }
    }
    void ReturnRope()
    {
        ropeOut = false;
        Destroy(projectileInst);
        rope = null;
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            if (!ropeOut)
            {
                ShootRope();
            }
            else
            {
                ReturnRope();
            }
        }
    }

    private void LateUpdate()
    {
        if (ropeOut && rope != null)
        {
            rope.Render(lineRenderer);
        }
    }

    private void FixedUpdate()
    {

        if (ropeOut && rope != null)
        {
            rope.PhysicsStep();
        }

    }
}

