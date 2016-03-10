using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	private bool isPaused = false;
	public GameHUD GHud;
    public MainMenuManager Options;
    public float speed = 2.0f;

    public GameObject mainHUD;
    public GameObject UIhud;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {                                // Pause must be on Update() if put on FixedUpdate()                                            
                                                   //Debug.Log(Time.timeScale);                  // The game will get stuck and will have to reset

        // if (InputManager.input.isPause){}
        //  if (InputManager.input.)

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {      // This will need to be changed to call inputManager  
			
			if(!isPaused) {                         
				pauseGame();
			} else if(isPaused) {
				unPauseGame();	
			}
		}

        if (InputManager.input.isActionPressed()) {

            Debug.Log("Pressing down");
            
       }

	}


    public void pauseGame()
    {                       // Pauses the game and brings up the Pause Menu
        isPaused = true;
        Time.timeScale = 0f;
        GHud.showPauseMenu();
        if (Options != null)
        {
            Options.mainCanvas.SetActive(false);
        }
	}

	public void unPauseGame() {                     // Unpauses the game and hides menu
		isPaused = false;
		Time.timeScale = 1f;
		GHud.hidePauseMenu();
	}
	public void unPauseGameBtt() {                  // When button is used to unpause this function is called
		isPaused = false;
		Time.timeScale = 1f;
		
	}

    public void setPause() {                        // It is called if game needs to be paused w/o Menu
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void setTimeNormal() {                   // It is called to set the time back to normal .. to unpause the game
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void manipulateTime(float speed) {       // Manipulate time if needed by getting a float and setting it
        Time.timeScale = speed;                     // to Time.timeScale ... Slows down Time or Speeds up Time
    }
    public void openOptions() {
        
       // UIhud.SetActive(false);
        mainHUD.SetActive(true);
     
        Options.GoToOptions();
    }
    }
