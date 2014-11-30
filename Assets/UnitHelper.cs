using UnityEngine;
using System.Collections;

public class UnitHelper : MonoBehaviour
{


	public static float KMPerHourToMetersPerSecond(float kmPerHour)
	{
		return kmPerHour * 1000f / (60f * 60f);
	}
	
	
	public static float KMPerHourToUnitsPerSecond(float kmPerHour)
	{
		float metersPerSecond = UnitHelper.KMPerHourToMetersPerSecond(kmPerHour);
		return metersPerSecond / ScaleManager.Instance.horizontalScale;
	}
}
