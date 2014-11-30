using UnityEngine;
using System.Collections;

public class Runway : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		Aircraft aircraft = other.GetComponent<Aircraft>();
		if (aircraft == null)
		{
			return;
		}

		float angle = Vector3.Angle(transform.forward, aircraft.transform.forward);

		bool headingOK = angle < 10f || angle > 170f;

		if (headingOK)
		{
			Destroy(aircraft.gameObject);
		}
	}
}
