using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour, IController
{
    public Rigidbody rigidBody;
    public Transform movementTransform;
    public Transform lookTransform;

    public class TransformState
    {
        public Vector3 position;
        public Vector3 rotation;

        public void SetFromTransform(Transform t)
        {
            SetPositionFromTransform(t);
            SetRotationFromTransform(t);
        }

        public void SetPositionFromTransform(Transform t)
        {
            position = t.position;
        }
        public void SetRotationFromTransform(Transform t)
        {
            rotation = t.eulerAngles;
        }

        public void Rotate(Vector3 _rotation)
        {
            rotation += _rotation;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(rotation) * translation;
            rotatedTranslation *= Time.deltaTime;
            position += rotatedTranslation;
        }

        public void LerpPosition(TransformState target, float positionLerpPct)
        {
            position = Vector3.Lerp(position, target.position, positionLerpPct);
        }
        public void LerpRotation(TransformState target, float rotationLerpPct)
        {
            rotation = Vector3.Lerp(rotation, target.rotation, rotationLerpPct);
        }


        public void LerpTowards(TransformState target, float positionLerpPct, float rotationLerpPct)
        {
            LerpPosition(target, positionLerpPct);
            LerpRotation(target, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = rotation;
            t.position = position;
        }
    }

    private TransformState targetState = new TransformState();
    private TransformState interpolatingState = new TransformState();

    public float rotationAcceleration;
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
        targetState.Rotate(input);
        interpolatingState.LerpRotation(targetState, rotationAcceleration);
        rigidBody.rotation = Quaternion.Euler(new Vector3(interpolatingState.rotation.x, interpolatingState.rotation.y));
        lookTransform.rotation = Quaternion.Euler(interpolatingState.rotation);
    }

    public void Move(Vector3 input)
    {
        targetState.SetPositionFromTransform(movementTransform);
        interpolatingState.SetPositionFromTransform(movementTransform);
        if (onGround)
        {
            targetState.Translate(input * moveSpeed);
            interpolatingState.LerpPosition(targetState, moveAcceleration);
            rigidBody.MovePosition(interpolatingState.position);

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
        if(rigidBody == null) rigidBody = GetComponent<Rigidbody>();
        if(movementTransform == null) movementTransform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        targetState.SetFromTransform(movementTransform);
        interpolatingState.SetFromTransform(movementTransform);
    }


    private void Update()
    {
        CheckOnGround();

    }
}
