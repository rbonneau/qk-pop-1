﻿using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	/*!This script is used so that prefabs using Unity UI buttons don't
	 * lose their functionality when dragged into a new scene.
	 * 
	 * The function naming convention is ClickButtonName_MenuTheButtonIsOn()
	 */

	public static ButtonController instane;

	GameHUD gHUD;
	PauseMenu pMenu;
	MasterSceneManager masterSceneManager;

	void Awake(){
		instane = this;
        gHUD = GameHUD.Instance;
		pMenu = PauseMenu.Instance;
		masterSceneManager = MasterSceneManager.Instance;
	}

	public void ClickJournalButton_PauseMenu(){
		gHUD.ShowJournal ();
	}

	public void ClickResumeButton_PauseMenu(){
		pMenu.unPauseGame ();
	}

	public void ClickMainMenu_PauseMenu(int levelIndex){
		pMenu.unPauseGame ();
		pMenu.showCursor ();
		masterSceneManager.loadLevelWithIndex (levelIndex);
	}

	public void ClickQuit_PauseMenu(){
		masterSceneManager.quitGame ();
	}

	public void ClickQuests_Journal(){
		gHUD.ShowQMUI ();
	}

	public void ClickSettings_Journal(){
		//Open settings menu when we have one
		return;
	}
}
