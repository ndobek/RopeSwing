using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour, IController
{
    public Rigidbody rigidBody;
    public Transform movementTransform;

    public Transform lookTransform;
    private float cameraPitch;
    private float cameraYaw;

    public float moveSpeed;
    public float sprintSpeed;
    //public float stepHeight;
    //public float maxSlope;

    private bool sprinting;
    public bool snappyAirControl = false;
    public float airControl;
    //public float drag;

    public float jumpHeight;

    public Transform groundChecker;
    public bool onGround;
    public LayerMask whatIsGround;

    public void Move(Vector3 input)
    {
        Vector3 newVelocity = rigidBody.velocity;
        if (onGround)
        {
            Vector3 v = Quaternion.Euler(0, lookTransform.rotation.eulerAngles.y, 0) * input;
            v *= sprinting ? sprintSpeed : moveSpeed;
            rigidBody.velocity = new Vector3(v.x, newVelocity.y, v.z);
        }
        else
        {
            Vector3 v = Quaternion.Euler(0, lookTransform.rotation.eulerAngles.y, 0) * input;
            v *= airControl;

            if(snappyAirControl) rigidBody.MovePosition(movementTransform.position + v);
            else rigidBody.velocity += v;
        }
    }

    public void Look(Vector3 input)
    {
        cameraPitch += input.x;
        cameraYaw += input.y;
        lookTransform.rotation = Quaternion.Euler(new Vector3(cameraPitch, cameraYaw, 0));
    }

    public void Jump()
    {
        if(onGround) rigidBody.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
    }

    public void CheckOnGround()
    {
        onGround = Physics.OverlapSphere(groundChecker.position, .2f, whatIsGround.value).Length > 0;
    }


    private void Awake()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody>();
        if (movementTransform == null) movementTransform = GetComponent<Transform>();
    }

    public void Sprint(bool onOff)
    {
        sprinting = onOff;
    }


    private void Update()
    {
        CheckOnGround();
    }
}
