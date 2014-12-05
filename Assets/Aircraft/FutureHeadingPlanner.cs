using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FutureHeadingPlanner : MonoBehaviour
{
	private Aircraft aircraft;

	public class FuturePlan
	{
		public float startDelay;
		public LinearPlanner linearPlanner;
		public float startHeading = 0f;

		public FuturePlan(float _startDelay, LinearPlanner _linearPlanner)
		{
			startDelay = _startDelay;
			linearPlanner = _linearPlanner;
		}
	}
	
	private List<FuturePlan> futurePlans = new List<FuturePlan>();
	

	void Awake()
	{
		aircraft = GetComponent<Aircraft>();
	}


	// Update is called once per frame
	void Update()
	{
		if (futurePlans.Count == 0)
		{
			return;
		}

		FuturePlan currentPlan = null;

		for (int i=0; i<futurePlans.Count; i++)
		{
			if (futurePlans[i].startDelay > 0)
			{
				break;
			}
			else
			{
				currentPlan = futurePlans[i];
			}
		}

		int numToRemove = futurePlans.IndexOf(currentPlan);
		if (numToRemove > 0)
		{
			Debug.Log("Removing " + numToRemove + " old plans (currently " + futurePlans.Count + ") at heading " + aircraft.CurrentHeadingInDegrees);
			futurePlans.RemoveRange(0, numToRemove);
			Debug.Log("Removed! There are now " + futurePlans.Count + " remaining.");
		}

		if (currentPlan != null && currentPlan.startDelay <= 0)
		{
			currentPlan.linearPlanner.UpdateCurrentValues(Time.deltaTime);
			aircraft.CurrentHeadingInDegrees = currentPlan.linearPlanner.CurrentValue;
		}

		foreach (FuturePlan futurePlan in futurePlans)
		{
			if (futurePlan != currentPlan)
			{
				futurePlan.startDelay -= Time.deltaTime;
			}
		}
	}


	public void AddPlan(float startDelay, float targetHeading)
	{
		Debug.Log("Creating plan at " + startDelay + " to go to " + targetHeading);

		int insertAt = 0;
		while (insertAt < futurePlans.Count)
		{
			float startTime = futurePlans[insertAt].startDelay;
			if (startTime > startDelay)
			{
				break;
			}
			else
			{
				insertAt++;
			}
		}

		float headingAtInsertTime = GetHeadingInFuture(startDelay);

		Debug.Log("Will be going at heading " + headingAtInsertTime);

		LinearPlanner planner = new LinearPlanner(headingAtInsertTime);
		planner.maxSpeed = aircraft.maxTurnSpeed;
		planner.acceleration = aircraft.turnAcceleration;
		planner.TargetValue = targetHeading;

		FuturePlan newPlan = new FuturePlan(startDelay, planner);
		newPlan.startHeading = headingAtInsertTime;

		futurePlans.Insert(insertAt, newPlan);

		UpdateAllPlans();
	}


	public void ModifyCurrentPlan(float targetHeading)
	{
		if (futurePlans.Count == 0)
		{
			AddPlan(0f, targetHeading);
		}
		else if (futurePlans[0].startDelay <= 0f)
		{
			//Debug.Log("Modifying current plan, but we already have a current plan!");
			futurePlans[0].linearPlanner.SetCurrentAndTarget(aircraft.CurrentHeadingInDegrees, targetHeading);
			futurePlans[0].linearPlanner.CreateNewPlan();
			UpdateAllPlans();
		}
		else
		{
			AddPlan(0f, targetHeading);
		}
	}


	private void UpdateAllPlans()
	{
		if (futurePlans.Count <= 1f)
		{
			return;
		}

		//Debug.Log("Updating all plans...");

		for (int i=0; i<futurePlans.Count; i++)
		{
			if (i == 0)
			{
				// Don't think we need to update the first one
			}
			else
			{
				float delay = futurePlans[i].startDelay;

				//Debug.Log("Plan " + i + " begins in " + delay + " seconds from now");

				float headingDelta = futurePlans[i].linearPlanner.TargetValue - futurePlans[i].linearPlanner.CurrentValue;
				float newStartHeading = GetHeadingInFuture(delay);
				float newTargetHeading = newStartHeading + headingDelta;

				//Debug.Log("Old start heading was " + futurePlans[i].linearPlanner.CurrentValue + ", new start heading is " + newStartHeading);

				futurePlans[i].linearPlanner.SetCurrentAndTarget(newStartHeading, newTargetHeading);
				futurePlans[i].linearPlanner.CreateNewPlan();
			}
		}
	}


	public float GetHeadingInFuture(float seconds)
	{
		//Debug.Log("Getting heading for " + seconds + " in future...");

		FuturePlan finalPlan = null;

		for (int i=0; i<futurePlans.Count; i++)
		{
			if (futurePlans[i].startDelay >= seconds)
			{
				break;
			}
			else
			{
				finalPlan = futurePlans[i];
			}
		}

		if (finalPlan != null)
		{
			//Debug.Log("Plan that we care about has a start delay of " + finalPlan.startDelay);

			float timeIntoPlan = seconds - finalPlan.startDelay;
			if (timeIntoPlan >= 0f)
			{
				return finalPlan.linearPlanner.GetValueInFuture(timeIntoPlan);
			}
		}

		//Debug.Log("Fuckit, just using my current heading");
		return aircraft.CurrentHeadingInDegrees;
	}


	public float GetCurrentTargetHeading()
	{
		if (futurePlans.Count > 0 && futurePlans[0].startDelay <= 0f)
		{
			return futurePlans[0].linearPlanner.TargetValue;
		}

		return aircraft.CurrentHeadingInDegrees;
	}


	public void SetCurrentTargetHeading(float targetHeading)
	{
		ModifyCurrentPlan(targetHeading);

		AddPlan(3f, 0f);
	}
}
