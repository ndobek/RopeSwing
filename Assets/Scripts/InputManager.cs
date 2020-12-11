using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public IController controller;
    public bool invertY;
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
        return direction;
    }

    private Vector3 GetInputRotation()
    {
        var mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

        var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

        mouseMovement *= mouseSensitivityFactor;

        return mouseMovement;
    }
    private void Awake()
    {
        controller = GetComponent<IController>();
    }
    public void Update()
    {
        controller.Move(GetInputTranslationDirection());
        controller.Look(GetInputRotation());
    }
}
