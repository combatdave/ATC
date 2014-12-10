using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider)), RequireComponent(typeof(Aircraft))]
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

		AircraftControllerUI ui = Instantiate(navigationUIPrefab, transform.position, transform.rotation) as AircraftControllerUI;
		ui.transform.SetParent(transform);
		ui.MyAircraft = GetComponent<Aircraft>();
		return ui;
	}
}
