using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    void Look(Vector3 input);
    void Move(Vector3 input);
}
