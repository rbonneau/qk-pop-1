using UnityEngine;
using System.Collections;

public enum ProgressType{ProgressGoal, CompleteGoal}

public class ProgressQuestAfterDialogue : MonoBehaviour {

	public bool ableToProgress;
	public int questID;
	public int goalIndex;
	public ProgressType progressType;
	//public bool newCompassTarget;
	//public GameObject newCompassTargetObject;

	/*!This function progresses the quest specified in the editor.
	 * This script should be attache to an NPC.
	 * If you want the NPC to progress a quest after the player finishes talking to it:
	 * 1. Check the Able To Progress toggle in the editor
	 * 2. Enter the ID of the quest and index of the goal in the editor
	 * 3. Select if you want to complete a goal or simply progress a goal in a quest
	 */
	public void ProgressQuest(){
		if (!ableToProgress) {return;}
		if (progressType == ProgressType.CompleteGoal) {
			QuestManager.instance.CompleteGoalInQuest (questID, goalIndex);
			QuestManager.instance.UpdateQuests();
			QuestManager.instance.LoadQuests();
		} else {
			QuestManager.instance.ProgressGoalInQuest(questID, goalIndex);
		}
	}
}
