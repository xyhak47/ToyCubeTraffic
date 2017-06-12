using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPath : MonoBehaviour
{
    private Material material;
    public float speed;
    public bool trigger;

    void Awake()
    {
        material = GetComponent<LineRenderer>().material;
    }

    void Start()
    {
        material.SetFloat("_Speed", speed);
        material.SetFloat("_Trigger", trigger ? 1 : 0);
    }
}
