using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public GameObject ChangelogPanel, ExtrasCanvas, CreditsCanvas, MainMenuCanvas, OptionsCanvas, PlayCanvas, PlayerCanvas;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPlayButtonPressed() {
        PlayCanvas.SetActive(!PlayCanvas.activeSelf);
        MainMenuCanvas.SetActive(!MainMenuCanvas.activeSelf);
        ChangelogPanel.SetActive(false);
        ExtrasCanvas.SetActive(!ExtrasCanvas.activeSelf);
        PlayerCanvas.SetActive(!PlayerCanvas.activeSelf);
    }

    public void OnOptionsButtonPressed() {
        OptionsCanvas.SetActive(!OptionsCanvas.activeSelf);
        MainMenuCanvas.SetActive(!MainMenuCanvas.activeSelf);
        ChangelogPanel.SetActive(false);
        ExtrasCanvas.SetActive(!ExtrasCanvas.activeSelf);
        PlayerCanvas.SetActive(!PlayerCanvas.activeSelf);
    }

    public void OnCreditsButtonPressed() {
        CreditsCanvas.SetActive(!CreditsCanvas.activeSelf);
        MainMenuCanvas.SetActive(!MainMenuCanvas.activeSelf);
        ChangelogPanel.SetActive(false);
        ExtrasCanvas.SetActive(!ExtrasCanvas.activeSelf);
        PlayerCanvas.SetActive(!PlayerCanvas.activeSelf);
    }

    public void OnChangelogButtonPressed() {
        ChangelogPanel.SetActive(!ChangelogPanel.activeSelf);
    }

    public void OnExitButtonPressed() {
        // Set up Cleanup functions to not lose or corrupt any data
        Application.Quit();
    }
}

