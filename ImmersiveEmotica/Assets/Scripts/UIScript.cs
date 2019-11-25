using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseScreen = GameObject.Find("PauseScreen");
        pauseMenu.SetActive(false);
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FOR MAIN MENU USE
    public void StartButton() {
        SceneManager.LoadScene("SadScene");
    }

    public void QuitButton() {
        Application.Quit(0);
    }

    // FOR SCENE USE
    public void RewindScene() {
        return;
    }

    public void PauseGame() {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        pauseMenu.SetActive(true);
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Menu");
    }
}
