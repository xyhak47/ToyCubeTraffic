﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	void Start ()
    {
		
	}

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 500);
    }
}
