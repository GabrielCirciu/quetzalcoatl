using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;
    public GameObject characterSelectionPanel, playModePanel;
    public GameObject hostButton, joinButton, singlePlayerPanel, hostPanel, joinPanel;

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
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
        singlePlayerPanel.SetActive(true);
    }

    public void OnMultiplayerButtonPressed() {
        hostButton.SetActive(true);
        joinButton.SetActive(true);
        singlePlayerPanel.SetActive(false);
        OnHostPanelButtonPressed();
    }

    public void OnHostPanelButtonPressed() {
        hostPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void OnJoinListButtonPressed() {
        joinPanel.SetActive(true);
        hostPanel.SetActive(false);
    }

    public void OnStartSinglePlayerButtonPressed() {
        SceneManager.LoadScene("CharacterAnimations");
    }

    public void OnStartServerHostButtonPressed() {
        // Nothing yet, start server here
    }

    public void OnJoinFriendButtonPressed() {
        // Nothing yet, join server here
    }
}
