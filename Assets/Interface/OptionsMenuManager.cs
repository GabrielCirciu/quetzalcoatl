using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;
    public GameObject videoPanel, audioPanel, gamePanel, controlPanel;

    void OnEnable() {
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        gamePanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            mainMenuManager.OnOptionsButtonPressed();
        }
    }

    public void OnVideoOptionsButtonPressed() {
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        gamePanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void OnAudioOptionsButtonPressed() {
        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
        gamePanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void OnGameOptionsButtonPressed() {
        gamePanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void OnControlOptionsButtonPressed() {
        controlPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void OnApplyButtonPressed() {
        //Will apply options, and save them
    }
}

