using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControls : MonoBehaviour {

    public GameObject pausePanel;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            CameraControls.fixCamera(pausePanel.activeSelf);
            return;
        }
    }


    public void ResumeButton()
    {
        pausePanel.SetActive(false);
        CameraControls.fixCamera(false);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
