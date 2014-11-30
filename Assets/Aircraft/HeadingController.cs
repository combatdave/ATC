using UnityEngine;
using System.Collections;

public class HeadingController : MonoBehaviour
{
	private LinearPlanner headingPlanner;
	private Aircraft aircraft;
	
	
	public float TargetHeadingInDegrees
	{
		get
		{
			return headingPlanner.TargetValue;
		}
		set
		{
			headingPlanner.TargetValue = value;
		}
	}
	
	
	void Awake()
	{
		aircraft = GetComponent<Aircraft>();
		headingPlanner = new LinearPlanner(aircraft.CurrentHeadingInDegrees);
		headingPlanner.maxSpeed = aircraft.maxTurnSpeed;
		headingPlanner.acceleration = aircraft.turnAcceleration;
		//headingPlanner.debug = true;
	}

	
	void Update()
	{
		headingPlanner.UpdateCurrentValues(Time.deltaTime);
		
		aircraft.CurrentHeadingInDegrees = headingPlanner.CurrentValue;
	}


	public float GetHeadingInFuture(float seconds)
	{
		return headingPlanner.GetValueInFuture(seconds);
	}
}
