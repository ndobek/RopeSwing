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

    public float rotateSpeed;
    public float moveSpeed;
    public float maxVelocityChange;
    public float maxAirSpeed;
    public float sprintSpeed;
    public float maxStepHeight;

    [SerializeField]
    private float stepCheckerResolution;

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
            Vector3 targetV = Quaternion.Euler(0, lookTransform.rotation.eulerAngles.y, 0) * input;
            targetV *= sprinting ? sprintSpeed : moveSpeed;
            //CheckStep(v, stepRayCastDistance);

            Vector3 vDiff = targetV - rigidBody.velocity;
            vDiff.x = Mathf.Clamp(vDiff.x, -maxVelocityChange, maxVelocityChange);
            vDiff.z = Mathf.Clamp(vDiff.z, -maxVelocityChange, maxVelocityChange);
            vDiff.y = 0;

            rigidBody.AddForce(vDiff, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 localdv = input *= airControl;
            Vector3 localv = lookTransform.InverseTransformVector(rigidBody.velocity);

            if (snappyAirControl)
            {
                Vector3 globaldv = Quaternion.Euler(0, lookTransform.rotation.eulerAngles.y, 0) * localdv;
                rigidBody.MovePosition(movementTransform.position + globaldv);
            }
            else
            {
                Vector3 v = new Vector3(localv.z, localv.y, localv.z);
                if (Mathf.Abs(localv.x + localdv.x) < maxAirSpeed) localv.x = Mathf.Clamp(localv.x + localdv.x, -maxAirSpeed, maxAirSpeed);

                //if (localv.y + localdv.y < maxAirSpeed) localv.y += localdv.y;
                if (Mathf.Abs(localv.z + localdv.z) < maxAirSpeed) localv.z = Mathf.Clamp(localv.z + localdv.z, -maxAirSpeed, maxAirSpeed);

                rigidBody.velocity = lookTransform.TransformVector(localv);
            }
        }
    }

    public void Look(Vector3 input)
    {
        input *= rotateSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch + input.x, -90, 90);
        cameraYaw += input.y;
        lookTransform.rotation = Quaternion.Euler(new Vector3(cameraPitch, cameraYaw, 0));
    }

    public void CheckStep(Vector3 point)
    {
        Vector3 pointToCheck = groundChecker.position;
        for (float offset = 0; offset < maxStepHeight; offset += maxStepHeight / stepCheckerResolution)
        {

        }

    }

    public void Jump()
    {
        if (onGround) rigidBody.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
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
