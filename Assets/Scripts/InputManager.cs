using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public IController controller;
    public bool invertY;
    public float mouseSensitivityMultiplier = 1;
    public AnimationCurve mouseSensitivityCurve;

    Vector3 GetInputTranslationDirection()
    {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            direction += Vector3.up;
        }
        return direction * Time.deltaTime;
    }

    private Vector3 GetInputRotation()
    {
        var mouseMovement = new Vector3(Input.GetAxis("Mouse Y") * (invertY ? 1 : -1), Input.GetAxis("Mouse X"));

        var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude) * mouseSensitivityMultiplier;

        mouseMovement *= mouseSensitivityFactor;

        return mouseMovement * Time.deltaTime;
    }
    private void Awake()
    {
        controller = GetComponent<IController>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) controller.Jump();
        controller.Sprint(Input.GetKey(KeyCode.LeftShift));
        controller.Move(GetInputTranslationDirection());
        controller.Look(GetInputRotation());
    }
}
