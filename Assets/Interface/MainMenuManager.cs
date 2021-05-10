using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject changelogPanel, extrasCanvas, creditsCanvas, mainMenuCanvas, optionsCanvas, playCanvas, playerInfoCanvas;

    private void Start() {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPlayButtonPressed() {
        playCanvas.SetActive(!playCanvas.activeSelf);
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        changelogPanel.SetActive(false);
        extrasCanvas.SetActive(!extrasCanvas.activeSelf);
        playerInfoCanvas.SetActive(!playerInfoCanvas.activeSelf);
    }

    public void OnOptionsButtonPressed() {
        optionsCanvas.SetActive(!optionsCanvas.activeSelf);
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        changelogPanel.SetActive(false);
        extrasCanvas.SetActive(!extrasCanvas.activeSelf);
        playerInfoCanvas.SetActive(!playerInfoCanvas.activeSelf);
    }

    public void OnCreditsButtonPressed() {
        creditsCanvas.SetActive(!creditsCanvas.activeSelf);
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        changelogPanel.SetActive(false);
        extrasCanvas.SetActive(!extrasCanvas.activeSelf);
        playerInfoCanvas.SetActive(!playerInfoCanvas.activeSelf);
    }

    public void OnChangelogButtonPressed() {
        changelogPanel.SetActive(!changelogPanel.activeSelf);
    }

    public void OnExitButtonPressed() {
        // Set up Cleanup functions to not lose or corrupt any data
        Application.Quit();
    }
}

