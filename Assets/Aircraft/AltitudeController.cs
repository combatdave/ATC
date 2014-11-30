using UnityEngine;
using System.Collections;

public class AltitudeController : MonoBehaviour
{
	private LinearPlanner altitudePlanner;
	private Aircraft aircraft;

	
	public float TargetAltitudeInMeters
	{
		get
		{
			return altitudePlanner.TargetValue;
		}
		set
		{
			altitudePlanner.TargetValue = value;
		}
	}


	void Awake()
	{
		aircraft = GetComponent<Aircraft>();
		altitudePlanner = new LinearPlanner(aircraft.CurrentAltitudeInMeters);
		altitudePlanner.maxSpeed = aircraft.maxVerticalSpeed;
		altitudePlanner.acceleration = aircraft.verticalAcceleration;
	}


	void Update()
	{
		altitudePlanner.UpdateCurrentValues(Time.deltaTime);

		aircraft.CurrentAltitudeInMeters = altitudePlanner.CurrentValue;
	}


	public float GetAltitudeInFuture(float seconds)
	{
		return altitudePlanner.GetValueInFuture(seconds);
	}
}
