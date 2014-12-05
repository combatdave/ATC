using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Aircraft))]
public class AircraftVisuals : MonoBehaviour
{
	public GameObject stick;
	public GameObject bottom;
	public TextMesh heightIndicator;
	public TextMesh targetHeightIndicator;
	public LineRenderer predictionLine;
	public LineRenderer groundPredictionLine;

	public int predictionSteps = 500;
	public float timeStepForPrediction = 0.05f;
	public float predictionLineWidth = 0.05f;


	private Aircraft aircraft;

	void Awake()
	{
		aircraft = GetComponent<Aircraft>();

		predictionLine.useWorldSpace = true;
		predictionLine.SetVertexCount(predictionSteps-1);
		predictionLine.SetWidth(predictionLineWidth, predictionLineWidth);
		predictionLine.SetColors(new Color(1f,1f,1f,1f), new Color(1f,1f,1f,0f));

		groundPredictionLine.useWorldSpace = true;
		groundPredictionLine.SetVertexCount(predictionSteps-1);
		groundPredictionLine.SetWidth(predictionLineWidth, predictionLineWidth);
		groundPredictionLine.SetColors(new Color(1f,1f,1f,1f), new Color(1f,1f,1f,0f));
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


		Vector3 p0 = transform.position;
		float speedInUnitsPerSecond = UnitHelper.KMPerHourToUnitsPerSecond(aircraft.speedInKMPerHour);
		for (int i=1; i<predictionSteps; i++)
		{
			float altitudeAtTime = GetComponent<AltitudeController>().GetAltitudeInFuture(i * timeStepForPrediction);
			float headingAtTime = GetComponent<HeadingController>().GetHeadingInFuture(i * timeStepForPrediction);
			
			Vector3 headingVector = Quaternion.Euler(new Vector3(0f, headingAtTime, 0f)) * Vector3.forward;
			
			Vector3 p1 = p0 + (headingVector * speedInUnitsPerSecond * timeStepForPrediction);
			p1.y = altitudeAtTime / ScaleManager.verticalScale;

			predictionLine.SetPosition(i-1, p1);

			Vector3 p1ground = p1;
			p1ground.y = 0.01f;

			groundPredictionLine.SetPosition(i-1, p1ground);

			p0 = p1;
		}
	}

}
