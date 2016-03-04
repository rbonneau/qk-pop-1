using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

public struct AI_Data
{

	private List<GameObject> paths;
    public float sightRange_;
    public float sightAngle_;


    public AI_Data(
        float SightRange,
        float SightAngle)
	{
		paths = new List<GameObject> ();
        sightRange_ = SightRange;
        sightAngle_ = SightAngle;
	}

	public void loadData(StatePatternEnemy target)
	{
        target.sightRange = sightRange_;
        target.sightAngle = sightAngle_;
	}
}

[CustomEditor(typeof(StatePatternEnemy), true)]
public class AIEditor : Editor {

	StatePatternEnemy ai_target;
	AnimBool show_data;
	string[] ai_types = new string[]{"Villager", "Guard", "Commander"};
	string[] path_types = new string[]{"one way", "loop around", "back and forth", "On Guard"};

	AI_Data[] ai_data = new AI_Data[]{
		new AI_Data(40f, 20f),
		new AI_Data(40f, 20f),
		new AI_Data(40f, 20f)};

	int ai_types_index = 0;
	int current_selection = 0;


	void OnEnable()
	{
		ai_target = (StatePatternEnemy)target;
		ai_types_index = ai_target.current_preset;
		show_data = new AnimBool(ai_target.customType);
		show_data.valueChanged.AddListener(Repaint);
    }

	override public void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("AI Type:", GUILayout.MaxWidth(60));
		ai_types_index = EditorGUILayout.Popup (ai_types_index, ai_types, GUILayout.MaxWidth(200));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Add Path", GUILayout.MaxWidth(100))){
			ai_target.Pathways.Add(null);
			ai_target.PathType.Add(0);
			ai_target.infinite.Add(false);
			ai_target.nofLoops.Add(1);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator ();

		for (int i=0; i < ai_target.Pathways.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			if(i < ai_target.Pathways.Count){
				ai_target.Pathways[i] = EditorGUILayout.ObjectField (ai_target.Pathways[i], typeof(GameObject), true, GUILayout.Width(80)) as GameObject; 
			}

			if(i < ai_target.Pathways.Count){
				GUILayout.Label("Loop Type:", GUILayout.MaxWidth(80));
				ai_target.PathType[i] = EditorGUILayout.Popup (ai_target.PathType[i], path_types, GUILayout.MaxWidth(100));
				GUILayout.Label("infinite?", GUILayout.MaxWidth(50));
				ai_target.infinite[i] = EditorGUILayout.Toggle(ai_target.infinite[i], GUILayout.MaxWidth(20));
				GUILayout.Label("number of loops", GUILayout.MaxWidth(90));
				ai_target.nofLoops[i] = EditorGUILayout.IntField(ai_target.nofLoops[i], GUILayout.MaxWidth(30));
			}
			if(GUILayout.Button("Remove Path")){
				ai_target.Pathways.RemoveAt(i);
				ai_target.PathType.RemoveAt(i);
				ai_target.infinite.RemoveAt(i);
				ai_target.nofLoops.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Show Data:", GUILayout.MaxWidth(70));
		show_data.target = EditorGUILayout.Toggle (show_data.target);
		ai_target.customType = show_data.target;
		EditorGUILayout.EndHorizontal ();

		if (EditorGUILayout.BeginFadeGroup (show_data.faded))
		{
            ai_target.sightRange = EditorGUILayout.FloatField("Sight Range:", ai_target.sightRange);
            ai_target.sightAngle = EditorGUILayout.FloatField("Sight Angle:", ai_target.sightAngle);



            /* old code
			ai_target.hp = EditorGUILayout.FloatField("Health:", ai_target.hp);
			ai_target.sightDistance = EditorGUILayout.FloatField("Sight Distance:", ai_target.sightDistance);
			ai_target.passiveSightAngle = EditorGUILayout.FloatField("Passive Sight Angle:", ai_target.passiveSightAngle);
			ai_target.chasingSightAngle = EditorGUILayout.FloatField("Chasing Sight Angle:", ai_target.chasingSightAngle);
			ai_target.speed = EditorGUILayout.FloatField("Speed:", ai_target.speed);
			ai_target.runSpeed = EditorGUILayout.FloatField("Running Speed:", ai_target.runSpeed);
			ai_target.attackDistance = EditorGUILayout.FloatField("Attack Distance:", ai_target.attackDistance);
			ai_target.suspicionLimit = EditorGUILayout.FloatField("Suspicion Limit:", ai_target.suspicionLimit);
			ai_target.aggressionLimit = EditorGUILayout.FloatField("Aggression Limit:", ai_target.aggressionLimit);
			ai_target.enemy = EditorGUILayout.Toggle("Aggressive:", ai_target.enemy);
            */
        }
        EditorGUILayout.EndFadeGroup();

		if (current_selection != ai_types_index)
		{
			loadAI();
			current_selection = ai_types_index;
			ai_target.current_preset = ai_types_index;
		}
	}

	private void loadAI()
	{
		if (ai_target.customType == false) {
			ai_data [ai_types_index].loadData (ai_target);
		}
	}

	private string print_array(string[] arr){
		string result = "";
		foreach(string s in arr){
			result += " \"";
			result += s;
			result += "\"";
		}
		return result;
	}
}
