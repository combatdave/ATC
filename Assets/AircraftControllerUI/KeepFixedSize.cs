using UnityEngine;
using System.Collections;

public class KeepFixedSize : MonoBehaviour 
{
	public float initialPixelHeight = 100f;
	private Vector3 initialScale; 


	void Start()
	{
		initialScale = transform.localScale; 
	}

	
	void LateUpdate() 
	{
		Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position); 
		float distance = plane.GetDistanceToPoint(transform.position);

		float frustumHeight = 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

		transform.localScale = initialScale * frustumHeight / initialPixelHeight; 
	}
}