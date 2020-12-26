using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Verlet attempt

//[System.Serializable]
//public class Rope
//{
//    [HideInInspector]
//    public float ropeLength;
//    public float ropeWidth;

//    public float mass = 1;
//    public float maxStretch;

//    public List<RopeSegment> ropeSegments = new List<RopeSegment>();
//    public int numberOfSegments;
//    public int numberOfSimulations;
//    public LayerMask collisionMask;
//    public bool collisions;

//    public RopeSegment Endpoint1 { get { return ropeSegments[0]; } }
//    public RopeSegment Endpoint2 { get { return ropeSegments[ropeSegments.Count - 1]; } }


//    public void CopySettings(Rope settings)
//    {
//        numberOfSegments = settings.numberOfSegments;
//        numberOfSimulations = settings.numberOfSimulations;
//        collisions = settings.collisions;
//        collisionMask = settings.collisionMask;
//        mass = settings.mass;
//    }
//    public Rope(Vector3 end1, Vector3 end2, Rope RopeSettings)
//    {
//        CopySettings(RopeSettings);
//        InitializeRope(end1, end2);
//    }


//    public void InitializeRope(Vector3 end1, Vector3 end2)
//    {
//        Vector3 currentPoint = end1;
//        Vector3 segmentVector = (end2 - end1) / numberOfSegments;
//        ropeLength = (end2 - end1).magnitude;
//        float segmentLength = ropeLength / numberOfSegments;

//        ropeSegments.Add(new RopeSegment(this, currentPoint));
//        for (int i = 0; i < numberOfSegments; i++)
//        {
//            currentPoint += segmentVector;
//            ropeSegments.Add(new RopeSegment(this, currentPoint, segmentLength));
//        }
//    }

//    public void Attach(Attachment obj, RopeSegment ropeSegment)
//    {
//        ropeSegment.attachment = obj;
//    }
//    public void Attach(Transform obj, bool lockPosition, RopeSegment ropeSegment)
//    {
//        Attach(new Attachment(obj, lockPosition), ropeSegment);
//    }

//    public class RopeSegment
//    {
//        public Rope rope;
//        public Vector3 PosCurrent;
//        public Vector3 PosPast;

//        public float Mass
//        {
//            get { return attachment == null ? rope.mass : rope.mass + attachment.Mass; }
//        }

//        public float SegmentLength;

//        public Attachment attachment;

//        private Vector3 GravityVector = new Vector3(0, -9.81f, 0);

//        public RopeSegment(Rope _rope, Vector3 pos, float segmentLength = 0)
//        {
//            rope = _rope;
//            PosCurrent = pos;
//            PosPast = pos;
//            SegmentLength = segmentLength;/* previousSegment == null ? 0 : (PosCurrent - previousSegment.PosCurrent).magnitude*/;
//        }

//        public void physicsStep()
//        {
//            Vector3 velocity = PosCurrent - PosPast;
//            PosPast = PosCurrent;
//            velocity += GravityVector * Time.deltaTime * Time.deltaTime;

//            Move(velocity);
//        }

//        public void Move(Vector3 movement)
//        {

//            if (rope.collisions)
//            {
//                Ray ray = new Ray(PosCurrent, movement);
//                RaycastHit hit;

//                if (Physics.Raycast(ray, out hit, movement.magnitude, rope.collisionMask))
//                {
//                    //movement = (PosCurrent - hit.point);
//                    //movement -= rope.ropeWidth * movement.normalized;
//                    movement = Vector3.zero;
//                };

//            }
//            //if (overrideSoftLock && attachment != null)
//            //{
//            //    attachment.MoveAttachment(movement);
//            //}

//            PosCurrent += movement;


//        }

//        public void ConstrainAttached()
//        {
//            if (attachment != null) attachment.ConstrainRope(this);
//        }

//        public static void AdjustDistance(RopeSegment obj1, RopeSegment obj2, float targetDistance)
//        {
//            Vector3 difference = (Vector3)obj1 - (Vector3)obj2;
//            Vector3 direction = difference.normalized;
//            float adjustmentDistance = difference.magnitude - targetDistance;

//            float adjustmentRatio1 = obj1.Mass / (obj1.Mass + obj2.Mass);
//            if (obj1.attachment != null && obj1.attachment.lockPosition) adjustmentRatio1 = 0;
//            float adjustmentRatio2 = 1 - adjustmentRatio1;

//            obj1.Move(-direction * adjustmentDistance * adjustmentRatio1);
//            obj2.Move(direction * adjustmentDistance * adjustmentRatio2);

//        }

//        public static explicit operator Vector3(RopeSegment obj) =>  obj.PosCurrent;
//    }

//    public class Attachment
//    {
//        public Transform transform;
//        public Rigidbody rigidbody;

//        public bool lockPosition;
//        public bool softLock;

//        public float Mass
//        {
//            get { return rigidbody == null ? 0 : rigidbody.mass; }
//        }

//        public Attachment(Transform _transform, Rigidbody _rigidbody, bool _lockPosition, bool _softLock)
//        {
//            transform = _transform;
//            rigidbody = _rigidbody;
//            lockPosition = _lockPosition;
//            softLock = _softLock;
//        }

//        public Attachment(Transform transform, bool lockPosition) : this(transform, null, lockPosition, false) { }


//        public void MoveAttachment(Vector3 movement)
//        {
//            if (!lockPosition && transform != null) transform.position += movement;
//        }

//        public void ConstrainRope(RopeSegment ropeSegment)
//        {
//            //if((ropeSegment.PosCurrent - transform.position).magnitude > ropeSegment.SegmentLength + (ropeSegment.SegmentLength * ropeSegment.rope.maxStretch) && softLock)
//            //{
//            //    MoveAttachment(ropeSegment.PosCurrent - transform.position);
//            //}
//            ropeSegment.PosCurrent = transform.position;
//        }
//    }

//    public void physicsStep()
//    {
//        if (ropeSegments != null && ropeSegments.Count > 0)
//        {
//            foreach (RopeSegment segment in ropeSegments)
//            {
//                segment.physicsStep();
//            }

//            for (int i = 0; i < numberOfSimulations; i++)
//            {
//                ApplyConstraints();
//            }
//        }
//    }

//    public void ApplyConstraints()
//    {
//        for(int i = 0; i < ropeSegments.Count - 1; i++)
//        {
//            RopeSegment.AdjustDistance(ropeSegments[i], ropeSegments[i + 1], ropeSegments[i + 1].SegmentLength);
//        }
//        for (int i = 0; i < ropeSegments.Count; i++)
//        {
//            ropeSegments[i].ConstrainAttached();
//        }
//    }

//    public Vector3[] GetPositions()
//    {
//        Debug.Log(ropeSegments.Count);
//        Vector3[] result = new Vector3[ropeSegments.Count];
//        for(int i = 0; i < result.Length; i++)
//        {
//            result[i] = (Vector3)ropeSegments[i];
//        }
//        return result;
//    }

//    public void Render(LineRenderer renderer)
//    {
//        if (ropeSegments != null && ropeSegments.Count > 0)
//        {
//            Vector3[] positions = GetPositions();
//            renderer.positionCount = positions.Length;
//            renderer.SetPositions(positions);
//        }
//    }
//}

#endregion


public class Rope
{

    public class RopeSegment
    {

    }

    public class Attachment
    {

    }
}