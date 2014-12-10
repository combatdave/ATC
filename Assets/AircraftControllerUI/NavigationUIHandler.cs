using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider)), RequireComponent(typeof(Aircraft))]
public class NavigationUIHandler : MonoBehaviour
{
	public string UICanvasTag = "UICanvas";
	public AircraftControllerUI navigationUIPrefab;
	private AircraftControllerUI navigationUI;
	

	public void ShowUI()
	{
		if (navigationUI == null)
		{
			navigationUI = SpawnUI();
		}

		navigationUI.SetShouldBeVisible(true);
	}


	public void HideUI()
	{
		if (navigationUI == null)
		{
			return;
		}

		navigationUI.SetShouldBeVisible(false);
	}


	private AircraftControllerUI SpawnUI()
	{
		GameObject parent = GameObject.FindGameObjectWithTag(UICanvasTag);

		if (parent != null)
		{
			AircraftControllerUI ui = Instantiate(navigationUIPrefab) as AircraftControllerUI;
			ui.transform.SetParent(parent.transform);
			ui.transform.localPosition = Vector3.zero;
			ui.transform.localScale = Vector3.one;
			ui.transform.localRotation = Quaternion.identity;
			ui.MyAircraft = GetComponent<AircraftBase>();
			return ui;
		}
		else
		{
			Debug.LogError("Couldn't find UI canvas with tag " + UICanvasTag + " when trying to spawn navigation UI");
		}

		return null;
	}
}
