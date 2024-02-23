using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Rope
{
    #region Rope Settings
    public float mass = .001f;
    public float slack = 0f;
    public float maxStretch = 1;

    public int numberOfSegments = 30;
    public int numberOfSimulations = 50;

    public bool collisions;
    public LayerMask collisionMask;

    #endregion
    [HideInInspector]
    public float ropeLength;

    #region Constructors and Initialization


    public Rope(Vector3 end1, Vector3 end2, Rope RopeSettings)
    {
        CopySettings(RopeSettings);
        InitializeRope(end1, end2);
    }

    public void CopySettings(Rope settings)
    {
        slack = settings.slack;
        maxStretch = settings.maxStretch;
        mass = settings.mass;
        numberOfSegments = settings.numberOfSegments;
        numberOfSimulations = settings.numberOfSimulations;
        collisions = settings.collisions;
        collisionMask = settings.collisionMask;
    }


    public void InitializeRope(Vector3 end1, Vector3 end2)
    {
        Vector3 currentPoint = end1;
        Vector3 segmentVector = (end2 - end1) / numberOfSegments;
        ropeLength = (end2 - end1).magnitude + slack;
        float segmentLength = ropeLength / numberOfSegments;

        ropePoints.Add(new RopePoint(this, currentPoint));
        for (int i = 0; i < numberOfSegments; i++)
        {
            currentPoint += segmentVector;
            ropePoints.Add(new RopePoint(this, currentPoint, segmentLength));
        }
    }

    public void Attach(Attachment obj, RopePoint ropeSegment)
    {
        ropeSegment.attachment = obj;
        obj.ropeSegment = ropeSegment;
    }
    public void Attach(GameObject obj, RopePoint ropeSegment)
    {
        Attach(new Attachment(obj), ropeSegment);
    }

    #endregion

    #region Rope Segments

    public List<RopePoint> ropePoints = new List<RopePoint>();
    public RopePoint Endpoint1 { get { return ropePoints[0]; } }
    public RopePoint Endpoint2 { get { return ropePoints[ropePoints.Count - 1]; } }

    public class RopePoint
    {
        #region Segment Info
        public float SegmentLength;
        private Vector3 GravityVector = new Vector3(0, -9.81f, 0);
        public Rope rope;
        public Vector3 PosCurrent;
        public Vector3 PosPast;

        public Attachment attachment;

        public Vector3 Velocity
        {
            get { return PosCurrent - PosPast; }
        }

        public float Mass
        {
            get { return attachment == null ? rope.mass : rope.mass + attachment.Mass; }
        }

        #endregion

        #region Constructors and Conversions

        public RopePoint(Rope _rope, Vector3 pos, float segmentLength = 0)
        {
            rope = _rope;
            PosCurrent = pos;
            PosPast = pos;
            SegmentLength = segmentLength;
        }
        public static explicit operator Vector3(RopePoint obj) => obj.PosCurrent;

        #endregion

        #region Physics Calculations

        public void MoveFromVelocity()
        {
            Vector3 velocity = PosCurrent - PosPast;
            PosPast = PosCurrent;
            velocity += GravityVector * Time.deltaTime * Time.deltaTime;
            Move(velocity);
        }

        public void Move(Vector3 movement)
        {
            if (rope.collisions)
            {
                // Ray ray = new Ray(PosCurrent, movement);
                // RaycastHit hit;

                // if (Physics.Raycast(ray, out hit, movement.magnitude + rope.ropeWidth, rope.collisionMask))
                // {
                //     movement = hit.point - PosCurrent;
                //     movement -= rope.ropeWidth * movement.normalized;
                // };

            }

            PosCurrent += movement;
        }

        public void ConstrainAttached()
        {
            if (attachment != null)
            {
                attachment.Move(PosCurrent - attachment.transform.position);
                PosCurrent = attachment.transform.position;
            }
        }

        #endregion


    }

    #endregion

    public class Attachment
    {
        #region Attached Bodies
        public Transform transform;
        public Rigidbody rigidbody;
        public RopePoint ropeSegment;

        #endregion

        public float Mass
        {
            get { return rigidbody == null ? 0 : rigidbody.mass; }
        }

        #region  Constructors

        public Attachment(Transform _transform, Rigidbody _rigidbody)
        {
            transform = _transform;
            rigidbody = _rigidbody;
        }
        public Attachment(GameObject gameObject)
        {
            transform = gameObject.transform;
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        #endregion

        public void Move(Vector3 movement)
        {
            if (rigidbody != null)
            {
                rigidbody.AddForce(movement, ForceMode.Impulse);
                // rigidbody.MovePosition(rigidbody.position + movement);
            }
        }
    }



    #region Physics Calculations

    public void AdjustDistance(RopePoint obj1, RopePoint obj2, float targetDistance)
    {
        Vector3 difference = (Vector3)obj1 - (Vector3)obj2;
        float distance = difference.magnitude;
        Vector3 direction = difference.normalized;
        float adjustmentDistance = distance - targetDistance;

        obj1.Move(-direction * adjustmentDistance * .5f);
        obj2.Move(direction * adjustmentDistance * .5f);
    }

    public void PhysicsStep()
    {
        if (ropePoints != null && ropePoints.Count > 0)
        {
            
            foreach (RopePoint point in ropePoints)
            {
                point.MoveFromVelocity();
            }

            for (int i = 0; i < numberOfSimulations; i++)
            {
                for (int j = 0; j < ropePoints.Count - 1; j++)
                {
                    AdjustDistance(ropePoints[j], ropePoints[j + 1], ropePoints[j + 1].SegmentLength);
                }


            }

            foreach (RopePoint point in ropePoints)
            {
                point.ConstrainAttached();

            }

        }
    }

    #endregion

    #region  Rendering

    public Vector3[] GetPositions()
    {
        Vector3[] result = new Vector3[ropePoints.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (Vector3)ropePoints[i];
        }
        return result;
    }

    public void Render(LineRenderer renderer)
    {
        if (ropePoints != null && ropePoints.Count > 0)
        {
            Vector3[] positions = GetPositions();
            renderer.positionCount = positions.Length;
            renderer.SetPositions(positions);
            DebugRopeLength();
        }
    }

    #endregion

    public bool IsOverLength()
    {
        return CurrentLength() <= ropeLength * maxStretch;
    }
    public float CurrentLength()
    {
        float currentLength = 0;
        for (int j = 0; j < ropePoints.Count - 1; j++)
        {
            currentLength += (ropePoints[j].PosCurrent - ropePoints[j + 1].PosCurrent).magnitude;

        }
        return currentLength;
    }

    #region Debug

    public void DebugRopeLength()
    {
        Debug.Log("Target Length: " + ropeLength + " Actual Length: " + CurrentLength());
    }

    #endregion

}
