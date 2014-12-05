﻿using UnityEngine;
using System.Collections;


public class PointAtCamera : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
	}
}
