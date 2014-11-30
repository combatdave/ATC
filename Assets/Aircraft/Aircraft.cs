using UnityEngine;
using System.Collections;


public class Aircraft : MonoBehaviour
{
	public float speedInKMPerHour = 330f;
	public float CurrentAltitudeInMeters
	{
		get
		{
			return transform.position.y * ScaleManager.Instance.verticalScale;
		}
		set
		{
			Vector3 pos = transform.position;
			pos.y = value / ScaleManager.Instance.verticalScale;
			transform.position = pos;
		}
	}
	public float CurrentHeadingInDegrees
	{
		get
		{
			return transform.rotation.eulerAngles.y;
		}
		set
		{
			transform.rotation = Quaternion.Euler(new Vector3(0f, value, 0f));
		}
	}

	public float verticalAcceleration = 2f; // m/s/s
	public float maxVerticalSpeed = 50f; // m/s

	public float maxTurnSpeed = 15f; // deg/s
	public float turnAcceleration = 5f; // deg/s/s
	

	public float TargetAltitudeInMeters
	{
		get
		{
			return GetComponent<AltitudeController>().TargetAltitudeInMeters;
		}
		set
		{
			GetComponent<AltitudeController>().TargetAltitudeInMeters = value;
		}
	}


	public float TargetHeadingInDegrees
	{
		get
		{
			return GetComponent<HeadingController>().TargetHeadingInDegrees;
		}
		set
		{
			GetComponent<HeadingController>().TargetHeadingInDegrees = value;
		}
	}
	


	// Update is called once per frame
	void Update()
	{
		Vector3 moveDir = transform.forward;
		moveDir.y = 0f;
		float speedInUnitsPerSecond = UnitHelper.KMPerHourToUnitsPerSecond(speedInKMPerHour);
		transform.position += moveDir * speedInUnitsPerSecond * Time.deltaTime;


		float timeStep = 0.05f;
		Vector3 p0 = transform.position;
		for (int i=0; i<1000; i++)
		{
			float altitudeAtTime = GetComponent<AltitudeController>().GetAltitudeInFuture(i * timeStep);
			float headingAtTime = GetComponent<HeadingController>().GetHeadingInFuture(i * timeStep);

			Vector3 headingVector = Quaternion.Euler(new Vector3(0f, headingAtTime, 0f)) * Vector3.forward;

			Vector3 p1 = p0 + (headingVector * speedInUnitsPerSecond * timeStep);
			p1.y = altitudeAtTime / ScaleManager.Instance.verticalScale;

			Debug.DrawLine(p0, p1);

			p0 = p1;
		}
	}
}
