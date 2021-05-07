using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour {
    public GameObject inGameMenuPanel, cinemachineCamera;
    public ChatManager chatManager;
    CinemachineFreeLook cameraFreeLook;

    public WorldManager worldManager;

    void Start() {
        cameraFreeLook = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        inGameMenuPanel.SetActive(false);
        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
    }

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            if ( !chatManager.isChatWindowOpen ) {
                inGameMenuPanel.SetActive(!inGameMenuPanel.activeSelf);
                if ( inGameMenuPanel.activeSelf ) {
                    Cursor.lockState = CursorLockMode.None;
                    cameraFreeLook.m_YAxis.m_MaxSpeed = 0f;
                    cameraFreeLook.m_XAxis.m_MaxSpeed = 0f;
                    cinemachineCamera.GetComponent<CameraControls>().enabled = false;
                }
                else OnResumeButtonPressed();
            }
            else chatManager.isChatWindowOpen = false;
        }
    }

    public void OnResumeButtonPressed() {
        Cursor.lockState = CursorLockMode.Locked;
        cameraFreeLook.m_YAxis.m_MaxSpeed = 3f;
        cameraFreeLook.m_XAxis.m_MaxSpeed = 1f;
        inGameMenuPanel.SetActive(false);
        cinemachineCamera.GetComponent<CameraControls>().enabled = true;
    }

    public void OnOptionsButtonPressed() {
        // Nothing yet
    }

    public void OnMainMenuButtonPressed() {
        worldManager.ReturnToMainMenu();
    }

    public void OnExitButtonPressed() {
        // Needs cleanup function
        Application.Quit();
    }
}
