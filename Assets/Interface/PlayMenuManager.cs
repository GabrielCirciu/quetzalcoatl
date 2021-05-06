using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;
    public GameObject characterSelectionPanel, playModePanel;
    public GameObject hostButton, joinButton, joinPanel, worldSelectionPanel;

    void OnEnable() {
        characterSelectionPanel.SetActive(true);
        playModePanel.SetActive(false);
    }

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) mainMenuManager.OnPlayButtonPressed();
    }

    public void OnToCharacterSelectionButtonPressed() {
        characterSelectionPanel.SetActive(true);
        playModePanel.SetActive(false);
    }

    public void OnToPlayModeSelectionButtonPressed() {
        characterSelectionPanel.SetActive(false);
        playModePanel.SetActive(true);
        OnSinglePlayerButtonPressed();
    }

    public void OnSinglePlayerButtonPressed() {
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        worldSelectionPanel.SetActive(true);
    }

    public void OnMultiplayerButtonPressed() {
        hostButton.SetActive(true);
        joinButton.SetActive(true);
        OnHostButtonPressed();
    }

    public void OnHostButtonPressed() {
        worldSelectionPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void OnJoinListButtonPressed() {
        joinPanel.SetActive(true);
        worldSelectionPanel.SetActive(false);
    }

    public void OnStartButtonPressed() {
        SceneManager.LoadScene("CharacterAnimations");
    }
}
