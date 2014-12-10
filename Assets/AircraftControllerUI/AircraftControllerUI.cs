using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityTools;


public class AircraftControllerUI : MonoBehaviour
{
	private AircraftBase _myAircraft;
	public AircraftBase MyAircraft
	{
		get { return _myAircraft; }
		set
		{
			_myAircraft = value;

			altitudeUI.SetTargetAltitude(MyAircraft.TargetAltitudeInMeters);
			headingUI.SetTargetHeading(MyAircraft.TargetHeadingInDegrees);
		}
	}


	public ChevronAnimator ascentDescentChevrons;

	public float maxAltitude = 12000f;
	public float minAltitude = 0f;

	public CanvasGroup horizontalNavigationCanvasGroup;
	public CanvasGroup altitudeDisplayCanvasGroup;
	

	public float becomeVisibleTime = 1f;
	private float currentVisibility = 0f;

	public HeadingUI headingUI;
	public AltitudeUI altitudeUI;


	private enum UIState
	{
		Hidden=0,
		Visible,
	}
	private UIState currentState = UIState.Hidden;


	void Awake()
	{
		SetCurrentVisibility(currentVisibility);
	}
	

	void Update()
	{
		if (MyAircraft == null)
		{
			Debug.Log("Aircraft for UI is null in update, destroying");
			Destroy(gameObject);
			return;
		}

		UpdateUIState();

		// Update the acent/descent chevrons
		ascentDescentChevrons.SetAscentAmount(MyAircraft.GetCurrentAltitudeChangeNormalized());

		headingUI.SetCurrentHeadingAndTarget(MyAircraft.CurrentHeadingInDegrees, MyAircraft.TargetHeadingInDegrees);
		altitudeUI.SetCurrentAltitude(MyAircraft.CurrentAltitudeInMeters);
	}


	void LateUpdate()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(MyAircraft.transform.position);
		RectTransform myRectTransform = GetComponent<RectTransform>();

		screenPos -= new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0f) / 2f;
		screenPos.z = 0f;

		myRectTransform.anchoredPosition = screenPos;
	}


	private void UpdateUIState()
	{
		float targetVisibility = 0f;
		if (currentState != UIState.Hidden)
		{
			targetVisibility = 1f;
		}

		if (currentVisibility != targetVisibility)
		{
			if (becomeVisibleTime > 0f)
			{
				currentVisibility = Mathf.MoveTowards(currentVisibility, targetVisibility, Time.deltaTime / becomeVisibleTime);
			}
			else
			{
				currentVisibility = targetVisibility;
			}
			SetCurrentVisibility(currentVisibility);
		}

		if (altitudeUI.CurrentState == AltitudeUI.State.input)
		{
			horizontalNavigationCanvasGroup.alpha = 0.2f;
		}
		else
		{
			horizontalNavigationCanvasGroup.alpha = 1f;
		}
	}


	public void SetShouldBeVisible(bool shouldBeVisible)
	{
		if (shouldBeVisible)
		{
			if (currentState == UIState.Hidden)
			{
				currentState = UIState.Visible;
			}
		}
		else
		{
			currentState = UIState.Hidden;
		}
	}


	private void SetCurrentVisibility(float visibility)
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		cg.alpha = visibility;

		cg.interactable = visibility > 0f;
		cg.blocksRaycasts = visibility > 0f;
	}
}
