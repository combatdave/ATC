using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class AircraftFarViewUI : MonoBehaviour
{
	public string UICameraTag = "UICamera";

	void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main; //GameObject.FindGameObjectWithTag(UICameraTag).GetComponent<Camera>();
	}


	public void OnIconClicked()
	{
		InputManager.Instance.SetSelectedAircraft(GetComponentInParent<Aircraft>());
	}


	void Update()
	{
		float alpha = 1f - FindObjectOfType<CameraController>().ZoomedInAmount;
		alpha = Mathf.Pow (alpha, 2f);

		foreach (Image i in GetComponentsInChildren<Image>())
		{
			Color c = i.color;
			c.a = alpha;
			i.color = c;
		}

		foreach (Button b in GetComponentsInChildren<Button>())
		{
			b.interactable = alpha > 0.5f;
		}
	}
}
