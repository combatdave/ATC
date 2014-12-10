using UnityEngine;
using System.Collections;


public class Aircraft : AircraftBase
{
	public float speedInKMPerHour = 330f;

	public override float CurrentAltitudeInMeters
	{
		get
		{
			return transform.position.y * ScaleManager.verticalScale;
		}
		set
		{
			Vector3 pos = transform.position;
			pos.y = value / ScaleManager.verticalScale;
			transform.position = pos;
		}
	}


	public override float TargetAltitudeInMeters
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


	public override float CurrentHeadingInDegrees
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
	

	public override float TargetHeadingInDegrees
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
	}


	public override float GetCurrentAltitudeChangeNormalized()
	{
		return GetComponent<AltitudeController>().GetCurrentSpeed() / maxVerticalSpeed;
	}
}
