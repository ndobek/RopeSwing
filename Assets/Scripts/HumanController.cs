using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanController : MonoBehaviour, IController
{
    [HideInInspector]
    public Rigidbody rb;
    public Transform t;

    public class TransformState
    {
        public Vector3 position;
        public Vector3 rotation;

        public void SetFromTransform(Transform t)
        {
            rotation = t.eulerAngles;
            position = t.position;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(rotation) * translation;
            position += rotatedTranslation;
        }

        public void LerpTowards(TransformState target, float positionLerpPct, float rotationLerpPct)
        {
            position = Vector3.Lerp(position, target.position, positionLerpPct);
            rotation = Vector3.Lerp(rotation, target.rotation, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = rotation;
            t.position = position;
        }
    }

    private TransformState targetState = new TransformState();
    private TransformState interpolatingState = new TransformState();

    public float moveAcceleration;
    public float moveSpeed;
    public float sprintSpeed;
    public float stepHeight;
    public float maxSlope;

    public float airMoveSpeed;
    public float drag;

    public float jumpHeight;

    public Transform groundChecker;
    public bool onGround;
    public LayerMask whatIsGround;

    private void Rotate(Vector3 input)
    {
        t.Rotate(input, Space.Self);
    }
    test
    public void Move(Vector3 input)
    {
        if (onGround)
        {
            targetState.SetFromTransform(t);
            targetState.Translate(input * moveSpeed);
            interpolatingState.LerpTowards(targetState, moveAcceleration, moveAcceleration);
            rb.AddRelativeForce(t.position - interpolatingState.position, ForceMode.VelocityChange);
        }
    }

    public void Look(Vector3 input)
    {
        Rotate(input);
    }

    public void CheckOnGround()
    {
        onGround = Physics.OverlapSphere(groundChecker.position, .2f, whatIsGround.value).Length > 0;
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        targetState.SetFromTransform(t);
        interpolatingState.SetFromTransform(t);
    }


    private void Update()
    {
        CheckOnGround();
    }
}
