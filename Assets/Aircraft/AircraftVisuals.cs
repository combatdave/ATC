using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Aircraft))]
public class AircraftVisuals : MonoBehaviour
{
	public GameObject stick;
	public GameObject bottom;
	public TextMesh heightIndicator;
	public TextMesh targetHeightIndicator;	


	private Aircraft aircraft;

	void Awake()
	{
		aircraft = GetComponent<Aircraft>();
	}


	// Update is called once per frame
	void Update()
	{
		Vector3 bottomPos = transform.position;
		bottomPos.y = 0.01f;

		bottom.transform.position = bottomPos;

		stick.transform.position = (transform.position + bottomPos) / 2f;
		Vector3 stickScale = stick.transform.localScale;
		stickScale.y = transform.position.y * 0.5f;
		stick.transform.localScale = stickScale;

		heightIndicator.text = InputManager.GetFormattedHeightString(aircraft.CurrentAltitudeInMeters);
		targetHeightIndicator.text = InputManager.GetFormattedHeightString(aircraft.TargetAltitudeInMeters);

		heightIndicator.transform.parent.rotation = Quaternion.LookRotation(-Vector3.right);
	}

}
