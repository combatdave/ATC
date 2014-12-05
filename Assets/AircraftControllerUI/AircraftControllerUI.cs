using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityTools;


public class AircraftControllerUI : MonoBehaviour
{
	private AircraftBase aircraft;

	public RectTransform inputHandle;
	public RectTransform currentTargetHandle;

	public Slider altitudeControlSlider;
	public Text targetAltitudeText;
	public Text currentAltitudeText;

	public ChevronAnimator ascentDescentChevrons;

	public float maxAltitude = 12000f;
	public float minAltitude = 0f;


	private float setAngle = 0f;

	public float becomeVisibleTime = 1f;
	private bool shouldBeVisible = false;
	private float currentVisibility = 0f;


	void Awake()
	{
		aircraft = GetComponentInParent<AircraftBase>();
		SetCurrentVisibility(currentVisibility);
	}
	

	void Start()
	{
		setAngle = 0f;

		OnAltitudeSliderChange();
	}


	void Update()
	{
		transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

		UpdateAltitudeSliderPosition();

		// Update the current altitude text
		currentAltitudeText.text = InputManager.GetFormattedHeightString(aircraft.CurrentAltitudeInMeters);

		// Update the acent/descent chevrons
		ascentDescentChevrons.SetAscentAmount(aircraft.GetCurrentAltitudeChangeNormalized());

		float targetVisibility = 0f;
		if (shouldBeVisible)
		{
			targetVisibility = 1f;
		}
		if (currentVisibility != targetVisibility)
		{
			currentVisibility = Mathf.MoveTowards(currentVisibility, targetVisibility, Time.deltaTime / becomeVisibleTime);
			SetCurrentVisibility(currentVisibility);
		}
	}


	public void SetShouldBeVisible(bool shouldBeVisible)
	{
		this.shouldBeVisible = shouldBeVisible;
	}


	private void SetCurrentVisibility(float visibility)
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		cg.alpha = visibility;
	}


#region UI Updaters

	private void UpdateInputHandle()
	{
		inputHandle.localRotation = Quaternion.Euler(0f, 0f, setAngle - 90f);
	}
	
	
	private void UpdateTargetHandle()
	{
		currentTargetHandle.localRotation = Quaternion.Euler(0f, 0f, setAngle - 90f);
	}


	void UpdateAltitudeSliderPosition()
	{
		RectTransform sliderTransform = altitudeControlSlider.GetComponent<RectTransform>();
		float height = sliderTransform.rect.height;

		float altitudeRange = maxAltitude - minAltitude;

		float currentAltitude = aircraft.CurrentAltitudeInMeters;

		float altitudeFraction = Mathf.Clamp01(currentAltitude/altitudeRange);
		float sliderYPos = Mathf.Lerp(height/2f, -height/2f, altitudeFraction);

		sliderTransform.anchoredPosition = new Vector2(sliderTransform.anchoredPosition.x, sliderYPos);
	}


	void UpdateTargetAltitudeText()
	{
		float altitude = Mathf.Lerp (minAltitude, maxAltitude, altitudeControlSlider.normalizedValue);
		targetAltitudeText.text = InputManager.GetFormattedHeightString (altitude);
	}


	void SetAircraftTargetAltitudeFromSlider()
	{
		aircraft.TargetAltitudeInMeters = Mathf.Lerp (minAltitude, maxAltitude, altitudeControlSlider.normalizedValue);
	}

#endregion


#region UI Events

	public void OnDragHeadingHandle(BaseEventData e)
	{
		PointerEventData pointerEvent = e as PointerEventData;
		
		Vector3 screenRotPoint = Camera.main.WorldToScreenPoint(inputHandle.position);
		
		Vector2 mouseOffsetFromPivot = new Vector2(screenRotPoint.x, screenRotPoint.y) - pointerEvent.position;
		
		setAngle = Mathf.Atan2(mouseOffsetFromPivot.x, -mouseOffsetFromPivot.y) * Mathf.Rad2Deg;
		
		UpdateInputHandle();
	}


	public void OnEndDragHeadingHandle(BaseEventData e)
	{
		UpdateTargetHandle();
	}


	public void OnAltitudeSliderChange()
	{
		UpdateTargetAltitudeText();
	}


	public void OnAltitudeSliderEndDrag(BaseEventData e)
	{
		SetAircraftTargetAltitudeFromSlider();
	}

#endregion
}
