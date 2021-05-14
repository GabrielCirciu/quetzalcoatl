using UnityEngine;
using Cinemachine;

public class InGameMenuManager : MonoBehaviour {
    public GameObject inGameMenuPanel, cinemachineCamera;
    public ChatManager chatManager;
    private CinemachineFreeLook _cinemachineFreeLook;
    private CameraControls _cameraControls;
    public SteamManager steamManager;

    private void Start() {
        steamManager = SteamManager.instance;
        _cinemachineFreeLook = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        _cameraControls = cinemachineCamera.GetComponent<CameraControls>();
        inGameMenuPanel.SetActive(false);
    }

    private void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            if ( !chatManager.isChatWindowOpen ) {
                inGameMenuPanel.SetActive(!inGameMenuPanel.activeSelf);
                if ( inGameMenuPanel.activeSelf ) {
                    Cursor.lockState = CursorLockMode.None;
                    _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0f;
                    _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0f;
                    _cameraControls.enabled = false;
                }
                else OnResumeButtonPressed();
            }
            else chatManager.isChatWindowOpen = false;
        }
    }

    public void OnResumeButtonPressed() {
        Cursor.lockState = CursorLockMode.Locked;
        _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0.01f;
        _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 1f;
        inGameMenuPanel.SetActive(false);
        _cameraControls.enabled = true;
    }

    public void OnOptionsButtonPressed() {
        // Nothing yet
    }

    public void OnMainMenuButtonPressed() {
        steamManager.LeaveSteamSocketServer();
    }

    public void OnExitButtonPressed() {
        // Needs cleanup function
        Application.Quit();
    }
}
