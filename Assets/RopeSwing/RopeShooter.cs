using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooter : MonoBehaviour
{
    public Rope ropeSettings;
    [HideInInspector]
    public Rope rope;
    [SerializeField]
    private GameObject projectilePrefab;
    [HideInInspector]
    private GameObject projectileInst;
    public Rigidbody rb;
    public LineRenderer lineRenderer;
    public LayerMask swingableSurfaces;
    public float maxDistance;
    public int mouseButton;

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(mouseButton))
        {
            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2));
            if (Physics.Raycast(ray, out hit, maxDistance, swingableSurfaces))
            {
                if(projectileInst == null) projectileInst = GameObject.Instantiate(projectilePrefab);
                projectileInst.transform.position = hit.point;
                rope = new Rope(projectileInst.transform.position, transform.position, ropeSettings);
                rope.Attach(projectileInst.transform, true, rope.Endpoint1);
                rope.Attach(new Rope.Attachment(rb.transform, rb, false), rope.Endpoint2);

            }
        }

    }

    private void FixedUpdate()
    {
        if (rope != null)
        {
            rope.physicsStep();
            rope.Render(lineRenderer);
        }
    }
}


