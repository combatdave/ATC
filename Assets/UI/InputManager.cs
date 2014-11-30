using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class InputManager : Singleton<InputManager>
{
	public Aircraft Selected {get; private set;}
	public GameObject aircraftSettingsPanel;
	public Text targetHeightText;
	public Text targetHeadingText;


	void Awake()
	{
		aircraftSettingsPanel.SetActive(Selected != null);
	}


	void Update()
	{
		if (Selected != null)
		{
			targetHeightText.text = InputManager.GetFormattedHeightString(Selected.TargetAltitudeInMeters);
			targetHeadingText.text = InputManager.GetFormattedHeadingString(Selected.TargetHeadingInDegrees);
		}
	}


	public void SetSelectedAircraft(Aircraft aircraft)
	{
		Selected = aircraft;

		aircraftSettingsPanel.SetActive(Selected != null);
	}


	public void IncreaseTargetAltitude()
	{
		if (Selected == null)
		{
			return;
		}

		Selected.TargetAltitudeInMeters += 100f;
	}


	public void DecreaseTargetAltitude()
	{
		if (Selected == null)
		{
			return;
		}

		Selected.TargetAltitudeInMeters -= 100f;
	}


	public void IncreaseTargetHeading()
	{
		if (Selected == null)
		{
			return;
		}
		
		Selected.TargetHeadingInDegrees += 45f;
	}
	
	
	public void DecreaseTargetHeading()
	{
		if (Selected == null)
		{
			return;
		}
		
		Selected.TargetHeadingInDegrees -= 45f;
	}


	public static string GetFormattedHeightString(float height)
	{
		if (height < 1000f)
		{
			float rounding = 10f;
			return string.Format("{0}m", Mathf.Round(height / rounding) * rounding);
		}
		
		height = height / 1000f;
		return string.Format("{0:F1} km", height);
	}


	public static string GetFormattedHeadingString(float heading)
	{
		heading = heading % 360f;
		float rounding = 10f;
		return string.Format("{0}*", Mathf.Round(heading / rounding) * rounding);
	}
}
