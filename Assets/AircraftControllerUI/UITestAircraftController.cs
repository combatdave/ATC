using UnityEngine;
using System.Collections;

public class UITestAircraftController : AircraftBase
{
	public override float CurrentAltitudeInMeters{get; set;}
	public override float CurrentHeadingInDegrees{get; set;}
	
	public override float TargetAltitudeInMeters{get; set;}
	public override float TargetHeadingInDegrees{get; set;}


	void Update()
	{
		CurrentAltitudeInMeters = Mathf.MoveTowards(CurrentAltitudeInMeters, TargetAltitudeInMeters, maxVerticalSpeed * Time.deltaTime);
		CurrentHeadingInDegrees = Mathf.MoveTowards(CurrentHeadingInDegrees, TargetHeadingInDegrees, maxTurnSpeed * Time.deltaTime);
	}


	public override float GetCurrentAltitudeChangeNormalized()
	{
		float dif = TargetAltitudeInMeters - CurrentAltitudeInMeters;

		if (Mathf.Abs(dif) > 100f)
		{
			return Mathf.Sign(dif);
		}
		else
		{
			return dif / 100f;
		}
	}
}

