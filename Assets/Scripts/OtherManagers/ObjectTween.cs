using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTween : MonoBehaviour
{
    public Vector3 Origin { get; private set; }
    private void Start()
    {
        Origin = transform.position;
    }
}
