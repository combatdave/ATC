using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public float worldSize;
	public float maxAltitude;
	public float minAltitude;

	public GameObject prefab;


	// Use this for initialization
	void Start ()
	{
		for (int i=0; i<6; i++)
		{
			Vector2 xz = Random.insideUnitCircle * worldSize / ScaleManager.horizontalScale;
			float altitude = Random.Range(minAltitude, maxAltitude);
			float heading = Random.value * 360f;
			
			Vector3 pos = new Vector3(xz.x, altitude / ScaleManager.verticalScale, xz.y);
			Quaternion rot = Quaternion.Euler(new Vector3(0f, heading, 0f));

			GameObject spawned = GameObject.Instantiate(prefab, pos, rot) as GameObject;

			//spawned.transform.position;
		}
	}
}
