using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Rope
{
    #region Rope Settings
    public float mass;


    public int numberOfSegments;
    public int numberOfSimulations;

    #endregion
    [HideInInspector]
    public float ropeLength;

    #region Rope Segments

    public List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public RopeSegment Endpoint1 { get { return ropeSegments[0]; } }
    public RopeSegment Endpoint2 { get { return ropeSegments[ropeSegments.Count - 1]; } }

    public class RopeSegment
    {
        #region Segment Info
        public float SegmentLength;
        private Vector3 GravityVector = new Vector3(0, -9.81f, 0);
        public Rope rope;
        public Vector3 PosCurrent;
        public Vector3 PosPast;
        public Attachment attachment;


        public float Mass
        {
            get { return attachment == null ? rope.mass : rope.mass + attachment.Mass; }
        }

        #endregion

        #region Constructors and Conversions

        public RopeSegment(Rope _rope, Vector3 pos, float segmentLength = 0)
        {
            rope = _rope;
            PosCurrent = pos;
            PosPast = pos;
            SegmentLength = segmentLength;
        }
        public static explicit operator Vector3(RopeSegment obj) => obj.PosCurrent;

        #endregion

        #region Physics Calculations

        public void physicsStep()
        {
            Vector3 velocity = PosCurrent - PosPast;
            PosPast = PosCurrent;
            velocity += GravityVector * Time.deltaTime * Time.deltaTime;
            Move(velocity);
        }
        public void Move(Vector3 movement)
        {
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

        #endregion

        public float Mass
        {
            get { return rigidbody == null ? 0 : rigidbody.mass; }
        }

        #region  Constructors

        public Attachment(Transform _transform, Rigidbody _rigidbody, bool _lockPosition)
        {
            transform = _transform;
            rigidbody = _rigidbody;
        }
        public Attachment(GameObject gameObject, bool _lockPosition)
        {
            transform = gameObject.transform;
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        #endregion

        public void Move(Vector3 movement)
        {
            if (rigidbody != null)
            {
                rigidbody.AddForce(movement, ForceMode.VelocityChange);
            }
        }
    }

    #region Constructors and Initialization


    public Rope(Vector3 end1, Vector3 end2, Rope RopeSettings)
    {
        CopySettings(RopeSettings);
        InitializeRope(end1, end2);
    }

    public void CopySettings(Rope settings)
    {
        mass = settings.mass;
        numberOfSegments = settings.numberOfSegments;
        numberOfSimulations = settings.numberOfSimulations;

    }


    public void InitializeRope(Vector3 end1, Vector3 end2)
    {
        Vector3 currentPoint = end1;
        Vector3 segmentVector = (end2 - end1) / numberOfSegments;
        ropeLength = (end2 - end1).magnitude;
        float segmentLength = ropeLength / numberOfSegments;

        ropeSegments.Add(new RopeSegment(this, currentPoint));
        for (int i = 0; i < numberOfSegments; i++)
        {
            currentPoint += segmentVector;
            ropeSegments.Add(new RopeSegment(this, currentPoint, segmentLength));
        }
    }

    public void Attach(Attachment obj, RopeSegment ropeSegment)
    {
        ropeSegment.attachment = obj;
    }
    public void Attach(GameObject obj, bool lockPosition, RopeSegment ropeSegment)
    {
        Attach(new Attachment(obj, lockPosition), ropeSegment);
    }

    #endregion

    #region Physics Calculations

    public static void AdjustDistance(RopeSegment obj1, RopeSegment obj2, float targetDistance)
    {
        Vector3 difference = (Vector3)obj1 - (Vector3)obj2;
        Vector3 direction = difference.normalized;
        float adjustmentDistance = difference.magnitude - targetDistance;

        float adjustmentRatio1 = obj1.Mass / (obj1.Mass + obj2.Mass);
        float adjustmentRatio2 = 1 - adjustmentRatio1;

        obj1.Move(-direction * adjustmentDistance * adjustmentRatio2);
        obj2.Move(direction * adjustmentDistance * adjustmentRatio1);

    }

    public void physicsStep()
    {
        if (ropeSegments != null && ropeSegments.Count > 0)
        {
            foreach (RopeSegment segment in ropeSegments)
            {
                segment.physicsStep();
            }

            for (int i = 0; i < numberOfSimulations; i++)
            {
                for (int j = 0; j < ropeSegments.Count - 1; j++)
                {
                    AdjustDistance(ropeSegments[j], ropeSegments[j + 1], ropeSegments[j + 1].SegmentLength);

                }
            }

            foreach (RopeSegment segment in ropeSegments)
            {
                segment.ConstrainAttached();
            }
        }
    }

    #endregion

    #region  Rendering

    public Vector3[] GetPositions()
    {
        Vector3[] result = new Vector3[ropeSegments.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (Vector3)ropeSegments[i];
        }
        return result;
    }

    public void Render(LineRenderer renderer)
    {
        if (ropeSegments != null && ropeSegments.Count > 0)
        {
            Vector3[] positions = GetPositions();
            renderer.positionCount = positions.Length;
            renderer.SetPositions(positions);
        }
    }

    #endregion

}
