using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooterOld : MonoBehaviour
{
    public float spring;
    public float damper;
    public float minStretch;
    public float maxStretch;
    public float maxDistance;

    public Rope ropeSettings;
    [HideInInspector]
    public Rope rope;
    private SpringJoint joint;
    [HideInInspector]
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
            rope.Attach(projectileInst.transform, true, rope.Endpoint1);
            rope.Attach(rb.transform, false, rope.Endpoint2);

            float distance = Vector3.Distance(grapplePoint, rb.position);

            joint = rb.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            joint.spring = spring;
            joint.damper = damper;
            joint.minDistance = distance * minStretch;
            joint.maxDistance = distance * maxStretch;

        }
    }
    void ReturnRope()
    {
        ropeOut = false;
        Destroy(projectileInst);
        Destroy(joint);
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
        if (ropeOut && rope != null)
        {
            rope.physicsStep();
            rope.Render(lineRenderer);
        }
    }
}

