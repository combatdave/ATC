using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class AltitudeUI : MonoBehaviour
{
	public Text currentAltitudeText;
	public Text targetAltitudeText;
	public CanvasGroup buttonGroup;

	public enum State
	{
		display=0,
		input,
	}
	public State CurrentState {get; private set;}
	private bool shouldBeVisible;

	private float targetAltitude;
	private float modifyingTargetAltitude;


	public void SetCurrentAltitude(float altitude)
	{
		currentAltitudeText.text = InputManager.GetFormattedHeightString(altitude);
	}


	public void SetTargetAltitude(float altitude)
	{
		targetAltitude = altitude;
		modifyingTargetAltitude = altitude;
	}


	public void SetVisibility(bool visible)
	{
		shouldBeVisible = visible;
	}


	void Update()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		float targetVisibility = shouldBeVisible ? 0f : 1f;
		cg.alpha = Mathf.MoveTowards(cg.alpha, targetVisibility, 0.5f);

		if (CurrentState == State.display)
		{
			targetAltitudeText.text = InputManager.GetFormattedHeightString(targetAltitude);
			buttonGroup.alpha = 0f;
			buttonGroup.blocksRaycasts = false;
			buttonGroup.interactable = false;
		}
		else if (CurrentState == State.input)
		{
			targetAltitudeText.text = InputManager.GetFormattedHeightString(modifyingTargetAltitude);
			buttonGroup.alpha = 1f;
			buttonGroup.blocksRaycasts = true;
			buttonGroup.interactable = true;
		}
	}


	public void OnUpButtonClicked()
	{
		modifyingTargetAltitude += 500f;
	}


	public void OnDownButtonClicked()
	{
		modifyingTargetAltitude -= 500f;
	}


	public void OnOKButtonClicked()
	{
		targetAltitude = modifyingTargetAltitude;
		GetComponentInParent<AircraftControllerUI>().MyAircraft.TargetAltitudeInMeters = targetAltitude;
		CurrentState = State.display;
	}


	public void OnPanelClicked()
	{
		CurrentState = State.input;
	}
}
