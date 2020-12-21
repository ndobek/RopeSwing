using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeShooter : MonoBehaviour
{
    public Rope ropeSettings;
    [HideInInspector]
    public Rope rope;
    public LineRenderer lineRenderer;
    public LayerMask swingableSurfaces;
    public float maxDistance;

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2));
            if (Physics.Raycast(ray, out hit, maxDistance, swingableSurfaces)) rope = new Rope(transform.position, hit.point, ropeSettings);
        }
        if (rope != null)
        {
            rope.physicsStep();
            rope.Render(lineRenderer);
        }
    }
}
