//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//
//public class AircraftAltitudeFlightPlan : MonoBehaviour
//{
//	private Aircraft aircraft;
//	private float _targetAltitudeInMeters;
//	public float TargetAltitudeInMeters
//	{
//		get
//		{
//			return _targetAltitudeInMeters;
//		}
//		set
//		{
//			if (_targetAltitudeInMeters != value)
//			{
//				_targetAltitudeInMeters = value;
//				CreateNewFlightPlan();
//			}
//		}
//	}
//
//	struct AccelerationManeuver
//	{
//		public float duration;
//		public float direction;
//		public float finalSpeed;
//	}
//
//	private List<AccelerationManeuver> maneuvers = new List<AccelerationManeuver>();
//
//
//	void Awake()
//	{
//		aircraft = GetComponent<Aircraft>();
//	}
//
//
//	void Start()
//	{
//		TargetAltitudeInMeters = aircraft.CurrentAltitudeInMeters;
//	}
//
//
//	private void CreateNewFlightPlan(bool debug=false)
//	{
//		float timeToFullSpeedFromZero = aircraft.maxVerticalSpeed / aircraft.verticalAcceleration;
//		float timeToStopFromFullSpeed = timeToFullSpeedFromZero;
//		float timeToFullSpeedFromCurrent = (aircraft.maxVerticalSpeed - Mathf.Abs(aircraft.VerticalSpeed)) / aircraft.verticalAcceleration;
//		float timeToStopFromCurrent = Mathf.Abs(aircraft.VerticalSpeed) / aircraft.verticalAcceleration;
//
//		float distanceNeededToStopFromFullSpeed = Mathf.Abs( (0 - Mathf.Pow(aircraft.maxVerticalSpeed, 2f)) / (2f * aircraft.verticalAcceleration) );
//		float distanceNeededToFullSpeedFromZero = distanceNeededToStopFromFullSpeed;
//		float distanceNeededToStopFromCurrent = Mathf.Abs( (0 - Mathf.Pow(aircraft.VerticalSpeed, 2f)) / (2f * aircraft.verticalAcceleration) );
//		float distanceNeededToFullSpeedFromCurrent = Mathf.Abs( (Mathf.Pow(aircraft.maxVerticalSpeed, 2f) - Mathf.Pow(aircraft.VerticalSpeed, 2f)) / (2f * aircraft.verticalAcceleration) );
//
//		maneuvers.Clear();
//
//		float altitudeDifference = TargetAltitudeInMeters - aircraft.CurrentAltitudeInMeters;
//
//		float altitudeWhenStopped = aircraft.CurrentAltitudeInMeters;
//
//		bool wrongDirection = Mathf.Sign(altitudeDifference) != Mathf.Sign(aircraft.VerticalSpeed);
//		bool willOverShoot = !wrongDirection && (Mathf.Abs(altitudeDifference) < distanceNeededToStopFromCurrent);
//		if (wrongDirection || willOverShoot)
//		{
//			// We need a stopping maneuver
//			if (debug)
//			{
//				if (wrongDirection)
//				{
//					Debug.Log("Moving in wrong direction! Creating stopping maneuver.");
//				}
//				else if (willOverShoot)
//				{
//					Debug.Log("Will overshoot! Creating stopping maneuver.");
//				}
//			}
//			maneuvers.Add(new AccelerationManeuver() {duration=timeToStopFromCurrent, direction=-Mathf.Sign(aircraft.VerticalSpeed), finalSpeed=0f});
//
//			altitudeWhenStopped = aircraft.CurrentAltitudeInMeters + (aircraft.VerticalSpeed * timeToStopFromCurrent / 2f);
//		}
//		else
//		{
//			float timeSinceStopped = Mathf.Abs(aircraft.VerticalSpeed / aircraft.verticalAcceleration);
//			float distanceTraveledWhileAccelerating = (0f + aircraft.VerticalSpeed) * timeSinceStopped / 2f;
//			float initialAltitude = aircraft.CurrentAltitudeInMeters - distanceTraveledWhileAccelerating;
//			if (debug)
//			{
//				Debug.Log("Already moving in correct direction. Initial altitude was " + initialAltitude + "m " + timeSinceStopped + " seconds ago");
//			}
//			altitudeWhenStopped = initialAltitude;
//		}
//
//		altitudeDifference = TargetAltitudeInMeters - altitudeWhenStopped;
//
//		if (Mathf.Abs(altitudeDifference) < distanceNeededToFullSpeedFromZero + distanceNeededToStopFromFullSpeed)
//		{
//			if (debug)
//			{
//				Debug.Log("Not going to reach full speed.");
//			}
//			// Not going to reach full speed. We are somewhere inside the maneuver though.
//			float midAltitude = (TargetAltitudeInMeters + altitudeWhenStopped) / 2f;
//			float distanceToMidAltitude = midAltitude - altitudeWhenStopped;
//			float speedAtMidAltitude = Mathf.Sqrt(Mathf.Abs(2 * aircraft.verticalAcceleration * distanceToMidAltitude)) * Mathf.Sign(altitudeDifference);
//			float timeUntilMidAltitudeFromStopped = Mathf.Abs(2f * distanceToMidAltitude / speedAtMidAltitude);
//
//			if (maneuvers.Count == 0)
//			{
//				// No stopping manouver, need to figure out current position
//				bool isInAccelerationPart;
//				if (altitudeDifference > 0f)
//				{
//					// Going up. Below the mid altitude, we will be accelerating
//					isInAccelerationPart = aircraft.CurrentAltitudeInMeters < midAltitude;
//				}
//				else
//				{
//					// Going down. Above the mid altitude we accelerated.
//					isInAccelerationPart = aircraft.CurrentAltitudeInMeters > midAltitude;
//				}
//
//				if (isInAccelerationPart)
//				{
//					float accelerationTimeRemaining = Mathf.Abs(speedAtMidAltitude - aircraft.VerticalSpeed) / aircraft.verticalAcceleration;
//					if (debug)
//					{
//						Debug.Log("Continuing to accelerate for " + accelerationTimeRemaining + " seconds, then decelerating for " + timeUntilMidAltitudeFromStopped + " seconds.");
//					}
//					maneuvers.Add(new AccelerationManeuver() {duration=accelerationTimeRemaining, direction=Mathf.Sign(altitudeDifference), finalSpeed=speedAtMidAltitude});
//					maneuvers.Add(new AccelerationManeuver() {duration=timeUntilMidAltitudeFromStopped, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
//				}
//				else
//				{
//					if (debug)
//					{
//						Debug.Log("Past half way point, decelerating for " + timeUntilMidAltitudeFromStopped + " seconds.");
//					}
//					float decelerationTimeRemaining = Mathf.Abs(aircraft.VerticalSpeed) / aircraft.verticalAcceleration;
//					maneuvers.Add(new AccelerationManeuver() {duration=decelerationTimeRemaining, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
//				}
//			}
//			else
//			{
//				// Got a stopping maneuver, so we will start and stop at 0. Easy!
//				maneuvers.Add(new AccelerationManeuver() {duration=timeUntilMidAltitudeFromStopped, direction=Mathf.Sign(altitudeDifference), finalSpeed=speedAtMidAltitude});
//				maneuvers.Add(new AccelerationManeuver() {duration=timeUntilMidAltitudeFromStopped, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
//			}
//		}
//		else
//		{
//			if (maneuvers.Count == 0)
//			{
//				// No stopping maneuver, so go ahead
//
//				float direction = Mathf.Sign(altitudeDifference);
//				if (debug)
//				{
//					Debug.Log("Accelerating to " + aircraft.maxVerticalSpeed * direction + "m/s from current speed in " + timeToFullSpeedFromCurrent);
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=timeToFullSpeedFromCurrent, direction=direction, finalSpeed=aircraft.maxVerticalSpeed * direction});
//
//				float fullSpeedDistance = Mathf.Abs(altitudeDifference) - (distanceNeededToFullSpeedFromCurrent + distanceNeededToStopFromFullSpeed);
//				float fullSpeedTime = fullSpeedDistance / aircraft.maxVerticalSpeed;
//				if (debug)
//				{
//					Debug.Log("Speed will then be " + aircraft.maxVerticalSpeed * direction + "m/s for " + fullSpeedTime + " seconds");
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=fullSpeedTime, direction=0, finalSpeed=aircraft.maxVerticalSpeed * direction});
//
//				if (debug)
//				{
//					Debug.Log("Decelerating to " + 0f + "m/s in " + timeToStopFromFullSpeed);
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=timeToStopFromFullSpeed, direction=-direction, finalSpeed=0f});
//			}
//			else
//			{
//				float direction = Mathf.Sign(altitudeDifference);
//				if (debug)
//				{
//					Debug.Log("Accelerating to " + aircraft.maxVerticalSpeed * direction + "m/s from stopped in " + timeToFullSpeedFromZero);
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=timeToFullSpeedFromZero, direction=direction, finalSpeed=aircraft.maxVerticalSpeed * direction});
//				
//				float fullSpeedDistance = Mathf.Abs(altitudeDifference) - (distanceNeededToFullSpeedFromZero + distanceNeededToStopFromFullSpeed);
//				float fullSpeedTime = fullSpeedDistance / aircraft.maxVerticalSpeed;
//				if (debug)
//				{
//					Debug.Log("Speed will then be " + aircraft.maxVerticalSpeed * direction + "m/s for " + fullSpeedTime + " seconds");
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=fullSpeedTime, direction=0, finalSpeed=aircraft.maxVerticalSpeed * direction});
//
//				if (debug)
//				{
//					Debug.Log("Decelerating to " + 0f + "m/s in " + timeToStopFromFullSpeed);
//				}
//				maneuvers.Add(new AccelerationManeuver() {duration=timeToStopFromFullSpeed, direction=-direction, finalSpeed=0f});
//			}
//		}
//	}
//
//
//	void Update()
//	{
//		if (maneuvers.Count == 0)
//		{
//			return;
//		}
//
//		UpdateAircraft();
//
//		Vector3 p0 = transform.position;
//		for (int i = 1; i < 100; i++)
//		{
//			float secondsPerStep = 0.1f;
//			Vector3 p1 = p0 + transform.forward * UnitHelper.KMPerHourToUnitsPerSecond(aircraft.speedInKMPerHour) * secondsPerStep;
//
//			float timeInFuture = i * secondsPerStep;
//			p1.y = GetAltitudeInFuture(timeInFuture) / ScaleManager.Instance.verticalScale;
//
//			Color c = Color.blue;
//			Debug.DrawLine(p0, p1, c);
//
//			p0 = p1;
//		}
//	}
//
//
//	void UpdateAircraft()
//	{
//		AccelerationManeuver currentManeuver = maneuvers[0];
//		
//		float currentAcceleration = currentManeuver.direction * aircraft.verticalAcceleration;
//		
//		aircraft.VerticalSpeed += currentAcceleration * Time.deltaTime;
//		
//		currentManeuver.duration -= Time.deltaTime;
//		
//		maneuvers[0] = currentManeuver;
//		
//		if (currentManeuver.duration < 0f)
//		{
//			aircraft.VerticalSpeed = currentManeuver.finalSpeed;
//			maneuvers.RemoveAt(0);
//		}
//	}
//
//
//	float GetAltitudeInFuture(float secondsInFuture)
//	{
//		float futureAltitude = aircraft.CurrentAltitudeInMeters;
//		float verticalSpeedAtStartOfManeuver = aircraft.VerticalSpeed;
//
//		foreach (AccelerationManeuver maneuver in maneuvers)
//		{
//			float timeToUseOfDuration = Mathf.Min(maneuver.duration, secondsInFuture);
//			//float altitudeChangeFromManeuver = (verticalSpeedAtStartOfManeuver + maneuver.finalSpeed) * timeToUseOfDuration / 2f;
//			float altitudeChangeFromManeuver = verticalSpeedAtStartOfManeuver * timeToUseOfDuration + (Mathf.Pow(timeToUseOfDuration, 2f) * maneuver.direction * aircraft.verticalAcceleration * 0.5f);
//			futureAltitude += altitudeChangeFromManeuver;
//
//			secondsInFuture -= maneuver.duration;
//			verticalSpeedAtStartOfManeuver = maneuver.finalSpeed;
//
//			if (secondsInFuture < 0f)
//			{
//				break;
//			}
//		}
//
//		return futureAltitude;
//	}
//}
