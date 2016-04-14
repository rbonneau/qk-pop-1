/*











						This script has been automatically generated.
						Do not alter it, or your changes will be undone.














*/
using System.Collections.Generic;
public static class EventLibrary {

	public static Dictionary<string, System.Type> staticClasses = new Dictionary<string, System.Type> {
	};

	public static string[] staticClassesNice = new string[] {};

	public static Dictionary<string, System.Type> monoClasses = new Dictionary<string, System.Type> {
		{ "StatePatternEnemy", typeof(StatePatternEnemy) },
		{ "AIMain", typeof(AIMain) },
		{ "AIMainTrimmed", typeof(AIMainTrimmed) },
		{ "QuestNPC", typeof(QuestNPC) },
		{ "AudioManager", typeof(AudioManager) },
		{ "PlayerInventory", typeof(PlayerInventory) },
		{ "QuestManager", typeof(QuestManager) },
		{ "Crate", typeof(Crate) },
		{ "Enemy", typeof(Enemy) },
		{ "Rope", typeof(Rope) },
		{ "Well", typeof(Well) },
		{ "PlayerSaveManager", typeof(PlayerSaveManager) },
		{ "DialogueManager", typeof(DialogueManager) },
		{ "GameHUD", typeof(GameHUD) },
	};

	public static string[] monoClassesNice = new string[] { "StatePatternEnemy",  "AIMain",  "AIMainTrimmed",  "QuestNPC",  "AudioManager",  "PlayerInventory",  "QuestManager",  "Crate",  "Enemy",  "Rope",  "Well",  "PlayerSaveManager",  "DialogueManager",  "UI", };

	public static Dictionary<string, bool> instanceClasses = new Dictionary<string, bool> {
		{ "StatePatternEnemy", false},
		{ "AIMain", false},
		{ "AIMainTrimmed", false},
		{ "QuestNPC", false},
		{ "AudioManager", false},
		{ "PlayerInventory", false},
		{ "QuestManager", false},
		{ "Crate", false},
		{ "Enemy", false},
		{ "Rope", false},
		{ "Well", false},
		{ "PlayerSaveManager", false},
		{ "DialogueManager", false},
		{ "GameHUD", false},
	};

	public static Dictionary<string, string[]> library = new Dictionary<string, string[]> {
		{ "QuestNPCMethods", new string[] {"ResetLocation", } },
		{ "AudioManagerMethods", new string[] {"playMe", "playAudio", "muteAudio", "changeVol", "seeVol", } },
		{ "PlayerInventoryMethods", new string[] {"LoadInventory", "SaveInventory", } },
		{ "QuestManagerMethods", new string[] {"LoadQuests", "SaveQuests", "UpdateQuests", "CompleteGoalInQuest", "ProgressGoalInQuest", "AddQuest", } },
		{ "CrateMethods", new string[] {"TestCrateFunction", } },
		{ "PlayerSaveManagerMethods", new string[] {"SavePlayerLocation", "LoadPlayerLocation", } },
		{ "DialogueManagerMethods", new string[] {"Speak", } },
		{ "GameHUDMethods", new string[] {"UpdateObjectiveText", "MoveCompassTargetPoint", "TriggerDialoguer", } },
		{ "CrateFields", new string[] {"temp", } },
		{ "ItemFields", new string[] {"pushCounter", "pullCounter", "cutCounter", "soundThrowCounter", "stunCounter", "quincAffected", } },
	};

	public static Dictionary<string, string[]> libraryNice = new Dictionary<string, string[]> {
		{ "QuestNPCMethods", new string[] {"ResetLocation", } },
		{ "AudioManagerMethods", new string[] {"playMe", "playAudio", "muteAudio", "changeVol", "seeVol", } },
		{ "PlayerInventoryMethods", new string[] {"LoadInventory", "SaveInventory", } },
		{ "QuestManagerMethods", new string[] {"LoadQuests", "SaveQuests", "UpdateQuests", "CompleteGoalInQuest", "ProgressGoalInQuest", "AddQuest", } },
		{ "CrateMethods", new string[] {"test", } },
		{ "PlayerSaveManagerMethods", new string[] {"SavePlayerLocation", "LoadPlayerLocation", } },
		{ "DialogueManagerMethods", new string[] {"Speak", } },
		{ "GameHUDMethods", new string[] {"UpdateObjectiveText", "MoveCompassTargetPoint", "TriggerDialoguer", } },
		{ "CrateFields", new string[] {"temp", } },
		{ "ItemFields", new string[] {"Pushed X Times", "Pulled X Times", "Cut X Times", "Sound Thrown X Times", "Stunned X Times", "Affected by QuinC", } },
	};
}