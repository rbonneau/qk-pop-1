using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/*
 * AI Path Editor Script
 * 
 * UI Script for the AI Path class. It is used to manipulate the list of the checkpoints like removing
 * a checkpoint or moving it up or down.
 */

[CustomEditor(typeof(AIPath), true)]
public class AIPath_Editor : Editor {

	AIPath path_target;

	void OnEnable()
	{
		path_target = (AIPath)target;
	}
	override public void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical();

		if (GUILayout.Button ("Add Checkpoint"))
		{
			GameObject instance = Resources.Load("checkpoint") as GameObject;
			GameObject new_point = Instantiate(instance, path_target.transform.position, Quaternion.identity) as GameObject;
			path_target.addCheckpoint(new_point);
		}

		if (GUILayout.Button ("Clear List"))
		{
			path_target.clearList();
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical();

		drawPathObjects();

		EditorGUILayout.EndVertical();
	}

	void drawPathObjects(){
		List<GameObject> checkpoints = path_target.checkpoints;

		EditorGUILayout.LabelField("Checkpoints");
		int count = 1;
		foreach(GameObject c in checkpoints){
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(count.ToString(), GUILayout.Width(20));

			if(GUILayout.Button("Select Checkpoint")){
				Selection.activeGameObject = c;
			}

			if(GUILayout.Button("Remove Checkpoint")){
				DestroyImmediate(c);
				checkpoints.Remove(c);
			}

			if(GUILayout.Button("Move Up")){
				int index = checkpoints.FindIndex(a => a == c);
				if(index + 1 < checkpoints.Count){
					GameObject next = checkpoints[index + 1];
					checkpoints[index + 1] = c;
					checkpoints[index] = next;
				}
			}

			if(GUILayout.Button("Move Down")){
				int index = checkpoints.FindIndex(a => a == c);
				if(index - 1 >= 0){
					GameObject prev = checkpoints[index - 1];
					checkpoints[index - 1] = c;
					checkpoints[index] = prev;
				}
			}

			EditorGUILayout.EndHorizontal();
			count ++;
		}
		count = 1;
	}
}
