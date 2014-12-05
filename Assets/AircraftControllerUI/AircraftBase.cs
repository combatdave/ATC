using UnityEngine;
using System.Collections;


public abstract class AircraftBase : MonoBehaviour
{
	public abstract float CurrentAltitudeInMeters{get; set;}
	public abstract float TargetAltitudeInMeters{get; set;}
	public abstract float GetCurrentAltitudeChangeNormalized();

	public abstract float CurrentHeadingInDegrees{get; set;}
	public abstract float TargetHeadingInDegrees{get; set;}
	
	public float verticalAcceleration = 2f; // m/s/s
	public float maxVerticalSpeed = 50f; // m/s
	
	public float maxTurnSpeed = 15f; // deg/s
	public float turnAcceleration = 5f; // deg/s/s
}
