using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LinearPlanner
{
	private float _targetValue;
	public float TargetValue
	{
		get
		{
			return _targetValue;
		}
		set
		{
			if (_targetValue != value)
			{
				_targetValue = value;
				CreateNewPlan();
			}
		}
	}

	struct AccelerationPeriod
	{
		public float duration;
		public float direction;
		public float finalSpeed;
	}
	
	private List<AccelerationPeriod> accelerationPeriods = new List<AccelerationPeriod>();

	private float _currentValue;
	public float CurrentValue
	{
		get
		{
			return _currentValue;
		}
		set
		{
			if (_currentValue != value)
			{
				_currentValue = value;
				CreateNewPlan();
			}
		}
	}
	private float _currentSpeed;
	public float CurrentSpeed
	{
		get
		{
			return _currentSpeed;
		}
		set
		{
			if (_currentSpeed != value)
			{
				_currentSpeed = value;
				CreateNewPlan();
			}
		}
	}

	public float maxSpeed;
	public float acceleration;

	public bool debug = false;

	public LinearPlanner(float value)
	{
		_currentValue = value;
		_targetValue = value;
	}
	

	private void CreateNewPlan()
	{
		if (debug)
		{
			Debug.Log("Creating plan to go from " + CurrentValue + " to " + TargetValue);
		}

		float timeToFullSpeedFromZero = maxSpeed / acceleration;
		float timeToStopFromFullSpeed = timeToFullSpeedFromZero;
		float timeToFullSpeedFromCurrent = (maxSpeed - Mathf.Abs(CurrentSpeed)) / acceleration;
		float timeToStopFromCurrent = Mathf.Abs(CurrentSpeed) / acceleration;
		
		float distanceNeededToStopFromFullSpeed = Mathf.Abs( (0 - Mathf.Pow(maxSpeed, 2f)) / (2f * acceleration) );
		float distanceNeededToFullSpeedFromZero = distanceNeededToStopFromFullSpeed;
		float distanceNeededToStopFromCurrent = Mathf.Abs( (0 - Mathf.Pow(CurrentSpeed, 2f)) / (2f * acceleration) );
		float distanceNeededToFullSpeedFromCurrent = Mathf.Abs( (Mathf.Pow(maxSpeed, 2f) - Mathf.Pow(CurrentSpeed, 2f)) / (2f * acceleration) );
		
		accelerationPeriods.Clear();
		
		float altitudeDifference = TargetValue - CurrentValue;
		
		float altitudeWhenStopped = CurrentValue;
		
		bool wrongDirection = Mathf.Sign(altitudeDifference) != Mathf.Sign(CurrentSpeed);
		bool willOverShoot = !wrongDirection && (Mathf.Abs(altitudeDifference) < distanceNeededToStopFromCurrent);
		if (wrongDirection || willOverShoot)
		{
			// We need a stopping maneuver
			if (debug)
			{
				if (wrongDirection)
				{
					Debug.Log("Moving in wrong direction! Creating stopping maneuver.");
				}
				else if (willOverShoot)
				{
					Debug.Log("Will overshoot! Creating stopping maneuver.");
				}
			}
			accelerationPeriods.Add(new AccelerationPeriod() {duration=timeToStopFromCurrent, direction=-Mathf.Sign(CurrentSpeed), finalSpeed=0f});
			
			altitudeWhenStopped = CurrentValue + (CurrentSpeed * timeToStopFromCurrent / 2f);
		}
		else
		{
			float timeSinceStopped = Mathf.Abs(CurrentSpeed / acceleration);
			float distanceTraveledWhileAccelerating = CurrentSpeed * timeSinceStopped / 2f;
			float initialValue = CurrentValue - distanceTraveledWhileAccelerating;
			if (debug)
			{
				Debug.Log("Already moving in correct direction at " + CurrentSpeed + ". Initial value was " + initialValue + " at " + timeSinceStopped + " seconds ago");
			}
			altitudeWhenStopped = initialValue;
		}
		
		altitudeDifference = TargetValue - altitudeWhenStopped;
		
		if (Mathf.Abs(altitudeDifference) < distanceNeededToFullSpeedFromZero + distanceNeededToStopFromFullSpeed)
		{
			if (debug)
			{
				Debug.Log("Not going to reach full speed.");
			}
			// Not going to reach full speed. We are somewhere inside the maneuver though.
			float midAltitude = (TargetValue + altitudeWhenStopped) / 2f;
			float distanceToMidAltitude = midAltitude - altitudeWhenStopped;
			float speedAtMidAltitude = Mathf.Sqrt(Mathf.Abs(2 * acceleration * distanceToMidAltitude)) * Mathf.Sign(altitudeDifference);
			float timeUntilMidAltitudeFromStopped = Mathf.Abs(2f * distanceToMidAltitude / speedAtMidAltitude);
			
			if (accelerationPeriods.Count == 0)
			{
				// No stopping manouver, need to figure out current position
				bool isInAccelerationPart;
				if (altitudeDifference > 0f)
				{
					// Going up. Below the mid altitude, we will be accelerating
					isInAccelerationPart = CurrentValue < midAltitude;
				}
				else
				{
					// Going down. Above the mid altitude we accelerated.
					isInAccelerationPart = CurrentValue > midAltitude;
				}
				
				if (isInAccelerationPart)
				{
					float accelerationTimeRemaining = Mathf.Abs(speedAtMidAltitude - CurrentSpeed) / acceleration;
					if (debug)
					{
						Debug.Log("Continuing to accelerate for " + accelerationTimeRemaining + " seconds, then decelerating for " + timeUntilMidAltitudeFromStopped + " seconds.");
					}
					accelerationPeriods.Add(new AccelerationPeriod() {duration=accelerationTimeRemaining, direction=Mathf.Sign(altitudeDifference), finalSpeed=speedAtMidAltitude});
					accelerationPeriods.Add(new AccelerationPeriod() {duration=timeUntilMidAltitudeFromStopped, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
				}
				else
				{
					if (debug)
					{
						Debug.Log("Past half way point, decelerating for " + timeUntilMidAltitudeFromStopped + " seconds.");
					}
					float decelerationTimeRemaining = Mathf.Abs(CurrentSpeed) / acceleration;
					accelerationPeriods.Add(new AccelerationPeriod() {duration=decelerationTimeRemaining, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
				}
			}
			else
			{
				// Got a stopping maneuver, so we will start and stop at 0. Easy!
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeUntilMidAltitudeFromStopped, direction=Mathf.Sign(altitudeDifference), finalSpeed=speedAtMidAltitude});
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeUntilMidAltitudeFromStopped, direction=-Mathf.Sign(altitudeDifference), finalSpeed=0f});
			}
		}
		else
		{
			if (accelerationPeriods.Count == 0)
			{
				// No stopping maneuver, so go ahead
				
				float direction = Mathf.Sign(altitudeDifference);
				float distanceForWholeSequence = Mathf.Abs(TargetValue - CurrentValue);
				float distanceTravelledWhileAccelerating = Mathf.Abs((CurrentSpeed * timeToFullSpeedFromCurrent) + (0.5f * acceleration * direction * Mathf.Pow(timeToFullSpeedFromCurrent, 2f)));

				if (debug)
				{
					Debug.Log("Accelerating to " + maxSpeed * direction + " from current speed in " + timeToFullSpeedFromCurrent + " distance=" + distanceTravelledWhileAccelerating);
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeToFullSpeedFromCurrent, direction=direction, finalSpeed=maxSpeed * direction});

				float fullSpeedDistance = (distanceForWholeSequence - distanceTravelledWhileAccelerating) - distanceNeededToStopFromFullSpeed;
				float fullSpeedTime = Mathf.Abs(fullSpeedDistance / maxSpeed);
				if (debug)
				{
					Debug.Log("Speed will then be " + maxSpeed * direction + " for " + fullSpeedTime + " seconds, distance=" + fullSpeedDistance);
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=fullSpeedTime, direction=0, finalSpeed=maxSpeed * direction});
				
				if (debug)
				{
					Debug.Log("Decelerating to " + 0f + " in " + timeToStopFromFullSpeed + " seconds, distance=" + distanceNeededToStopFromFullSpeed);
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeToStopFromFullSpeed, direction=-direction, finalSpeed=0f});
			}
			else
			{
				float direction = Mathf.Sign(altitudeDifference);
				if (debug)
				{
					Debug.Log("Accelerating to " + maxSpeed * direction + " from stopped in " + timeToFullSpeedFromZero);
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeToFullSpeedFromZero, direction=direction, finalSpeed=maxSpeed * direction});
				
				float fullSpeedDistance = Mathf.Abs(altitudeDifference) - (distanceNeededToFullSpeedFromZero + distanceNeededToStopFromFullSpeed);
				float fullSpeedTime = fullSpeedDistance / maxSpeed;
				if (debug)
				{
					Debug.Log("Speed will then be " + maxSpeed * direction + " for " + fullSpeedTime + " seconds");
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=fullSpeedTime, direction=0, finalSpeed=maxSpeed * direction});
				
				if (debug)
				{
					Debug.Log("Decelerating to " + 0f + " in " + timeToStopFromFullSpeed);
				}
				accelerationPeriods.Add(new AccelerationPeriod() {duration=timeToStopFromFullSpeed, direction=-direction, finalSpeed=0f});
			}
		}
	}


	public float GetValueInFuture(float secondsInFuture)
	{
		float futureAltitude = CurrentValue;
		float verticalSpeedAtStartOfManeuver = CurrentSpeed;
		
		foreach (AccelerationPeriod accelerationPeriod in accelerationPeriods)
		{
			float timeToUseOfDuration = Mathf.Min(accelerationPeriod.duration, secondsInFuture);
			float altitudeChangeFromManeuver = verticalSpeedAtStartOfManeuver * timeToUseOfDuration + (Mathf.Pow(timeToUseOfDuration, 2f) * accelerationPeriod.direction * acceleration * 0.5f);
			futureAltitude += altitudeChangeFromManeuver;
			
			secondsInFuture -= accelerationPeriod.duration;
			verticalSpeedAtStartOfManeuver = accelerationPeriod.finalSpeed;
			
			if (secondsInFuture < 0f)
			{
				break;
			}
		}
		
		return futureAltitude;
	}


	public void UpdateCurrentValues(float deltaTime)
	{
		if (accelerationPeriods.Count == 0)
		{
//			float settleTime = 10f;
//			_currentSpeed = 0f;
//			_currentValue = Mathf.MoveTowards(_currentValue, _targetValue, maxSpeed * deltaTime / settleTime);
			return;
		}

		AccelerationPeriod currentAccelerationPeriod = accelerationPeriods[0];
		
		float currentAcceleration = currentAccelerationPeriod.direction * acceleration;
		
		_currentSpeed += currentAcceleration * deltaTime;
		_currentValue += CurrentSpeed * deltaTime;
		
		currentAccelerationPeriod.duration -= deltaTime;
		
		accelerationPeriods[0] = currentAccelerationPeriod;
		
		if (currentAccelerationPeriod.duration < 0f)
		{
			_currentSpeed = currentAccelerationPeriod.finalSpeed;
			accelerationPeriods.RemoveAt(0);
		}
	}
}
