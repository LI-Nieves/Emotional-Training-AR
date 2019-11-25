using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseScreen;
    public GameObject choicesUI;
    public GameObject winUI;
    public GameObject loseUI;

    public bool showChoices;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseScreen = GameObject.Find("PauseScreen");
        choicesUI = GameObject.Find("ChoicesUI");
        winUI = GameObject.Find("WinScreen");
        loseUI = GameObject.Find("LoseScreen");

        pauseMenu.SetActive(false);
        pauseScreen.SetActive(false);
        choicesUI.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);

        showChoices = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (choicesUI) {
            choicesUI.SetActive(showChoices);
        }
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
        SceneManager.LoadScene("SadScene");
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

    public void ShowChoicesUI() {
        showChoices = true;
    }

    public void HideChoicesUI() {
        showChoices = false;
    }

    // Functions for Choices
    public void CorrectChoice() {
        winUI.SetActive(true);
    }

    public void IncorrectChoice() {
        loseUI.SetActive(true);
    }
}
