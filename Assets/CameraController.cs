using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public float zoomInFOV = 10f;
	public float zoomOutFOV = 50f;
	public float zoomInDist = 100f;
	public float zoomOutDist = 400f;

	public float manualYawSpeed = 1f;
	public float manualPitchSpeed = 1f;
	public float manualZoomSpeed = 1f;
	public float manualMoveSpeed = 1f;

	new private Camera camera;

	public float ZoomedInAmount {get; set;}

	private Aircraft lastLookingAt = null;
	private bool doAutoZoom = false;


	// Use this for initialization
	void Awake()
	{
		camera = GetComponentInChildren<Camera>();
	}


	void Update()
	{
		if (Input.GetMouseButton(1))
		{
			// rmb
			float deltaYaw = Input.GetAxis("Mouse X") * manualYawSpeed;
			transform.RotateAround(transform.position, Vector3.up, deltaYaw);

			Transform pitchTransform = transform.GetChild(0);
			float currentPitch = pitchTransform.localRotation.eulerAngles.x;
			float deltaPitch = -Input.GetAxis("Mouse Y") * manualPitchSpeed;
			currentPitch += deltaPitch;
			currentPitch = Mathf.Clamp(currentPitch, 20f, 89f);
			pitchTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
		}
		else if (Input.GetMouseButton(2))
		{
			// mmb
			float xSpeed = Input.GetAxis("Mouse X") * manualMoveSpeed;
			float ySpeed = Input.GetAxis("Mouse Y") * manualMoveSpeed;

			if (Mathf.Abs(xSpeed) > float.Epsilon || Mathf.Abs(ySpeed) > float.Epsilon)
			{
				doAutoZoom = false;
				transform.position -= (transform.forward * ySpeed) + (transform.right * xSpeed);
			}
		}

		float scrollInput = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(scrollInput) > float.Epsilon)
		{
			doAutoZoom = false;

			float zoomSpeed = scrollInput * manualZoomSpeed;
			ZoomedInAmount = Mathf.Clamp01(ZoomedInAmount + zoomSpeed);
		}
	}

	
	// Update is called once per frame
	void LateUpdate()
	{
		Aircraft currentSelected = InputManager.Instance.Selected;
//		if (currentSelected != null && currentSelected != lastLookingAt)
//		{
//			doAutoZoom = true;
//		}
		lastLookingAt = currentSelected;

		float targetZoomedInAmount = ZoomedInAmount;
		if (doAutoZoom)
		{
			targetZoomedInAmount = 1f;
		}

		ZoomedInAmount = Mathf.Clamp01(Mathf.MoveTowards(ZoomedInAmount, targetZoomedInAmount, Time.deltaTime));

		camera.fieldOfView = Mathf.Lerp(zoomOutFOV, zoomInFOV, ZoomedInAmount);

		camera.transform.localPosition = -Vector3.forward * Mathf.Lerp(zoomOutDist, zoomInDist, ZoomedInAmount);

		if (InputManager.Instance.Selected != null && doAutoZoom)
		{
			Vector3 lookAtPos = Vector3.Lerp(Vector3.zero, InputManager.Instance.Selected.transform.position, ZoomedInAmount);
			transform.position = lookAtPos;
		}
	}
}
