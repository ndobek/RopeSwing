using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rope
{
    public float ropeWidth;

    public List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public int numberOfSegments;
    public int numberOfSimulations;
    public LayerMask collisionMask;
    public bool collisions;

    public Rope(Vector3 end1, Vector3 end2, int _numberOfSegments, int _numberOfSimulations, bool _collisions, LayerMask _collisionMask)
    {
        numberOfSegments = _numberOfSegments;
        numberOfSimulations = _numberOfSimulations;
        collisions = _collisions;
        collisionMask = _collisionMask;
        InitializeRope(end1, end2);
    }

    public Rope(Vector3 end1, Vector3 end2, int _numberOfSegments, int _numberOfSimulations) : this(end1, end2, _numberOfSegments, _numberOfSimulations, false, 0) { }

    public Rope(Vector3 end1, Vector3 end2, Rope RopeSettings) : this(end1, end2, RopeSettings.numberOfSegments, RopeSettings.numberOfSimulations, RopeSettings.collisions, RopeSettings.collisionMask) { }

    public void InitializeRope(Vector3 end1, Vector3 end2)
    {
        Vector3 currentPoint = end1;
        Vector3 segmentVector = (end2 - end1) / numberOfSegments;
        Debug.Log("End1:" + end1);
        Debug.Log("End2:" + end2);
        Debug.Log("Segment Vector:" + segmentVector);

        ropeSegments.Add(new RopeSegment(this, currentPoint, true));
        for (int i = 0; i < numberOfSegments; i++)
        {
            currentPoint += segmentVector;
            ropeSegments.Add(new RopeSegment(this, currentPoint, false, ropeSegments[i]));
        }
        ropeSegments.Add(new RopeSegment(this, currentPoint, true));
    }

    public class RopeSegment
    {
        public Rope rope;
        public Vector3 PosCurrent;
        public Vector3 PosPast;

        public float SegmentLength;

        public bool locked;


        private Vector3 GravityVector = new Vector3(0, -9.81f, 0);

        public RopeSegment(Rope _rope, Vector3 pos, bool _locked, RopeSegment previousSegment = null)
        {
            rope = _rope;
            PosCurrent = pos;
            PosPast = pos;
            locked = _locked;
            SegmentLength = previousSegment == null ? 0 : (PosCurrent - previousSegment.PosCurrent).magnitude;
        }

        public void physicsStep()
        {
            Vector3 velocity = PosCurrent - PosPast;
            PosPast = PosCurrent;
            velocity += GravityVector * Time.deltaTime * Time.deltaTime;
            Move(velocity);
        }

        public void Move(Vector3 movement)
        {
            if (!locked)
            {
                if (rope.collisions)
                {
                    Ray ray = new Ray(PosCurrent, movement);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, movement.magnitude, rope.collisionMask))
                    {
                        //movement = (PosCurrent - hit.point);
                        //movement -= rope.ropeWidth * movement.normalized;
                        //movement = Vector3.zero;
                    };

                }
                PosCurrent += movement;
            }
        }

        public static void AdjustDistance(RopeSegment obj1, RopeSegment obj2, float distance)
        {
            Vector3 difference = (Vector3)obj1 - (Vector3)obj2;
            Vector3 direction = difference.normalized;
            float adjustmentDistance = (difference.magnitude - distance) * .5f;

            obj2.Move(direction * adjustmentDistance);
            obj1.Move(-direction * adjustmentDistance);
        }

        public static explicit operator Vector3(RopeSegment obj) => obj.PosCurrent;
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
                ApplyConstraints();
            }
        }
    }

    public void ApplyConstraints()
    {
        for(int i = 0; i < ropeSegments.Count - 1; i++)
        {
            RopeSegment.AdjustDistance(ropeSegments[i], ropeSegments[i + 1], ropeSegments[i + 1].SegmentLength);
        }
    }

    public Vector3[] GetPositions()
    {
        Debug.Log(ropeSegments.Count);
        Vector3[] result = new Vector3[ropeSegments.Count];
        for(int i = 0; i < result.Length; i++)
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
}
