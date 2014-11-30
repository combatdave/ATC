using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(Spawner))] 
public class SpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Spawner myTarget = target as Spawner;
		//myTarget.MyValue = EditorGUILayout.IntSlider("Val-you", myTarget.MyValue, 1, 10);
	}


	void OnSceneGUI()
	{
		Spawner myTarget = target as Spawner;

		Handles.color = Color.red;
		Handles.DrawWireDisc(myTarget.transform.position, Vector3.up, 20f);
	}
}
