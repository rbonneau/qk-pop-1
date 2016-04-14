using UnityEngine;
using System.Collections;

public class Goal {

	string goalName;
	int goalProgress = -1;
	int goalProgressNeeded;
	bool completed = false;

	public Goal(string name) {
		goalName = name;
	}

	public Goal(string name, int progressNeeded) {
		goalName = name;
		goalProgress = 0;
		goalProgressNeeded = progressNeeded;
	}

	public Goal(string name, int progressNeeded, int progressCompleted) {
		goalName = name;
		goalProgress = progressCompleted;
		goalProgressNeeded = progressNeeded;
	}

	public void Complete() {

		if (goalProgress != -1) {
			goalProgress = goalProgressNeeded;
		}
		completed = true;

		//Debug.Log ("Goal Completed");
		return;
	}

	public bool IsCompleted() {
		return completed;
	}

	public string GetName() {
		return goalName;
	}

	public int GetProgress() {
		return goalProgress;
	}

	public int GetProgressNeeded(){
		return goalProgressNeeded;
	}

	public void Progress() {
		if (goalProgress == -1) {
			Debug.Log("No progress available on goal! Use Complete on this goal!");
			return;
		}

		goalProgress++;
		Debug.Log (goalProgress + "/" + goalProgressNeeded + " on goal progress");
		if (goalProgress >= goalProgressNeeded) {
			Debug.Log("Goal Complete");
			completed = true;
		}

		return;
	}
}
