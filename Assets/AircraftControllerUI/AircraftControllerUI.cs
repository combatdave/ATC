using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityTools;


public class AircraftControllerUI : MonoBehaviour
{
	public AircraftBase MyAircraft{get; set;}

	public RectTransform inputHandle;
	public RectTransform currentTargetHandle;
	public RectTransform currentHeadingHandle;
	
	public Text targetAltitudeText;
	public Text currentAltitudeText;

	public Image angleDifferenceDisplay;

	public ChevronAnimator ascentDescentChevrons;

	public float maxAltitude = 12000f;
	public float minAltitude = 0f;

	public CanvasGroup horizontalNavigationCanvasGroup;
	public CanvasGroup altitudeDisplayCanvasGroup;


	private float targetHeadingAngle = 0f;

	public float becomeVisibleTime = 1f;
	private float currentVisibility = 0f;

	public AltitudeUI altitudeUI;


	private enum UIState
	{
		Hidden=0,
		Visible,
	}
	private UIState currentState = UIState.Hidden;


	void Awake()
	{
		MyAircraft = GetComponentInParent<AircraftBase>();
		SetCurrentVisibility(currentVisibility);
	}
	

	void Start()
	{
		targetHeadingAngle = MyAircraft.TargetHeadingInDegrees;
		UpdateHeadingInputHandle();
		UpdateHeadingTargetHandle();

		altitudeUI.SetTargetAltitude(MyAircraft.TargetAltitudeInMeters);
	}


	void Update()
	{
		transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

		// Update the acent/descent chevrons
		ascentDescentChevrons.SetAscentAmount(MyAircraft.GetCurrentAltitudeChangeNormalized());

		currentHeadingHandle.localRotation = Quaternion.Euler(0f, 0f, -MyAircraft.CurrentHeadingInDegrees - 90f);

		UpdateUIState();

		altitudeUI.SetCurrentAltitude(MyAircraft.CurrentAltitudeInMeters);

		float fillAmount = (MyAircraft.TargetHeadingInDegrees - MyAircraft.CurrentHeadingInDegrees) / 360f;


		angleDifferenceDisplay.fillAmount = Mathf.Abs(fillAmount);
		angleDifferenceDisplay.fillClockwise = fillAmount < 0f;
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
			currentVisibility = Mathf.MoveTowards(currentVisibility, targetVisibility, Time.deltaTime / becomeVisibleTime);
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
	}


#region UI Updaters

	private void UpdateHeadingInputHandle()
	{
		inputHandle.localRotation = Quaternion.Euler(0f, 0f, -targetHeadingAngle - 90f);
	}
	
	
	private void UpdateHeadingTargetHandle()
	{
		currentTargetHandle.localRotation = Quaternion.Euler(0f, 0f, -targetHeadingAngle - 90f);
	}


	void SetAircraftTargetHeading()
	{
		float angleDifference = targetHeadingAngle - MyAircraft.CurrentHeadingInDegrees;

		if (angleDifference < -180)
		{
			angleDifference += 360f;
		}
		else if (angleDifference > 180f)
		{
			angleDifference -= 360f;
		}

		MyAircraft.TargetHeadingInDegrees = MyAircraft.CurrentHeadingInDegrees + angleDifference;
	}

#endregion


#region UI Events

	public void OnDragHeadingHandle(BaseEventData e)
	{
		PointerEventData pointerEvent = e as PointerEventData;

		Vector3 uiCenterPoint = inputHandle.position;
		Ray mouseRay = Camera.main.ScreenPointToRay(pointerEvent.position);
		Plane plane = new Plane(Vector3.up, uiCenterPoint);
		float dist = 0f;
		if (plane.Raycast(mouseRay, out dist))
		{
			Vector3 worldMousePos = mouseRay.origin + (mouseRay.direction * dist);

			Vector2 mouseOffset = new Vector2(worldMousePos.x - uiCenterPoint.x, worldMousePos.z - uiCenterPoint.z);
			targetHeadingAngle = -Mathf.Atan2(-mouseOffset.x, mouseOffset.y) * Mathf.Rad2Deg;

			UpdateHeadingInputHandle();
		}
	}


	public void OnEndDragHeadingHandle(BaseEventData e)
	{
		UpdateHeadingTargetHandle();
		SetAircraftTargetHeading();
	}


#endregion
}
