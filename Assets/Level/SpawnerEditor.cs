using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(Spawner))] 
public class SpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		//Spawner myTarget = target as Spawner;
		//myTarget.MyValue = EditorGUILayout.IntSlider("Val-you", myTarget.MyValue, 1, 10);
	}


	void OnSceneGUI()
	{
		Spawner spawner = target as Spawner;

		Handles.color = Color.red;

		Vector3 minHeight = Vector3.up * spawner.minAltitude / ScaleManager.verticalScale;
		Handles.DrawWireDisc(spawner.transform.position + minHeight, Vector3.up, spawner.worldSize / ScaleManager.horizontalScale);

		Vector3 maxHeight = Vector3.up * spawner.maxAltitude / ScaleManager.verticalScale;
		Handles.DrawWireDisc(spawner.transform.position + maxHeight, Vector3.up, spawner.worldSize / ScaleManager.horizontalScale);
	}
}
