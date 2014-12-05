using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class NavigationUIHandler : MonoBehaviour
{
	public AircraftControllerUI navigationUIPrefab;
	private AircraftControllerUI navigationUI;


	void OnMouseUpAsButton()
	{
		if (navigationUI == null)
		{
			navigationUI = SpawnUI();
		}

		navigationUI.SetShouldBeVisible(true);
	}


	private AircraftControllerUI SpawnUI()
	{
		AircraftControllerUI existingUI = GetComponentInChildren<AircraftControllerUI>();
		if (existingUI != null)
		{
			return existingUI;
		}

		GameObject uiGO = Instantiate(navigationUIPrefab, transform.position, transform.rotation) as GameObject;
		uiGO.transform.SetParent(transform);
		return uiGO.GetComponent<AircraftControllerUI>();
	}
}
