using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HeadingUI : MonoBehaviour
{
	public RectTransform targetModifierTransform;
	public RectTransform currentTargetTransform;
	public RectTransform currentHeadingTransform;
	
	public Image angleDifferenceDisplay;

	private float targetHeadingAngle = 0f;


	void Start()
	{
		UpdateHeadingInputHandle();
		UpdateHeadingTargetHandle();
	}


	void Update()
	{
		transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.forward);
	}


	public void SetTargetHeading(float targetHeading)
	{
		targetHeadingAngle = targetHeading;
	}


	public void SetCurrentHeadingAndTarget(float heading, float target)
	{
		currentHeadingTransform.localRotation = Quaternion.Euler(0f, 0f, -heading - 90f);
		
		float fillAmount = (target - heading) / 360f;
		angleDifferenceDisplay.fillAmount = Mathf.Abs(fillAmount);
		angleDifferenceDisplay.fillClockwise = fillAmount < 0f;
	}


	private void UpdateHeadingInputHandle()
	{
		targetModifierTransform.localRotation = Quaternion.Euler(0f, 0f, -targetHeadingAngle - 90f);
	}
	
	
	private void UpdateHeadingTargetHandle()
	{
		currentTargetTransform.localRotation = Quaternion.Euler(0f, 0f, -targetHeadingAngle - 90f);
	}


	void SetAircraftTargetHeading()
	{
		AircraftBase aircraft = GetComponentInParent<AircraftControllerUI>().MyAircraft;

		float angleDifference = targetHeadingAngle - aircraft.CurrentHeadingInDegrees;
		
		if (angleDifference < -180)
		{
			angleDifference += 360f;
		}
		else if (angleDifference > 180f)
		{
			angleDifference -= 360f;
		}
		
		aircraft.TargetHeadingInDegrees = aircraft.CurrentHeadingInDegrees + angleDifference;
	}


	public void OnDragHeadingHandle(BaseEventData e)
	{
		PointerEventData pointerEvent = e as PointerEventData;

		AircraftBase aircraft = GetComponentInParent<AircraftControllerUI>().MyAircraft;
		Vector3 uiCenterPoint = aircraft.transform.position;
		Ray mouseRay = Camera.main.ScreenPointToRay(pointerEvent.position);
		Plane plane = new Plane(Vector3.up, uiCenterPoint);
		float dist = 0f;
		if (plane.Raycast(mouseRay, out dist))
		{
			Vector3 worldMousePos = mouseRay.origin + (mouseRay.direction * dist);
			
			Vector2 mouseDir = new Vector2(worldMousePos.x - uiCenterPoint.x, worldMousePos.z - uiCenterPoint.z).normalized;
			Vector2 worldNorth = new Vector2(0f, 1f);
			Vector2 worldEast = new Vector2(1f, 0f);

			float angle = Vector2.Angle(worldNorth, mouseDir);
			float dir = Mathf.Sign(Vector2.Dot(worldEast, mouseDir));

			targetHeadingAngle = angle * dir;

			UpdateHeadingInputHandle();
		}
	}
	
	
	public void OnEndDragHeadingHandle(BaseEventData e)
	{
		UpdateHeadingTargetHandle();
		SetAircraftTargetHeading();
	}
}
