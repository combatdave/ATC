using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Aircraft))]
public class AircraftInput : MonoBehaviour
{
	void OnMouseUpAsButton()
	{
		InputManager.Instance.SetSelectedAircraft(GetComponent<Aircraft>());
	}
}
