using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class ButtonControl : MonoBehaviour {

    // Load a scene based on its index in the build settings
    public void setScene(int scene)
    {
		Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    // For the quit button exit the game
    public void quitGame()
    {
        Application.Quit();
    }

    // Possibly not needed
    public void loadArena(int arena)
    {
        //Level.arenaToLoad = arena;
    }

    // Set the number of players in the Level script
    public void setNumPlayers(int num)
    {
        Level.numplayers = num;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
