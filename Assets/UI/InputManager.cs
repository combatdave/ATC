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
		if (Selected != null)
		{
			Selected.GetComponent<NavigationUIHandler>().HideUI();
		}

		Selected = aircraft;

		Selected.GetComponent<NavigationUIHandler>().ShowUI();

		aircraftSettingsPanel.SetActive(Selected != null);
	}


	public void DeselectAircraft()
	{
		SetSelectedAircraft(null);
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
		
		Selected.TargetHeadingInDegrees += 90f;
	}
	
	
	public void DecreaseTargetHeading()
	{
		if (Selected == null)
		{
			return;
		}
		
		Selected.TargetHeadingInDegrees -= 90;
	}


	public static string GetFormattedHeightString(float height)
	{
		if (height < 1000f)
		{
			float rounding = 10f;
			return string.Format("{0}M", Mathf.Round(height / rounding) * rounding);
		}
		
		height = height / 1000f;
		return string.Format("{0:F1}KM", height);
	}


	public static string GetFormattedHeadingString(float heading)
	{
		heading = heading % 360f;
		float rounding = 10f;
		return string.Format("{0}*", Mathf.Round(heading / rounding) * rounding);
	}
}
