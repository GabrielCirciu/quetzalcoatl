using UnityEngine;

public class PlayMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;
    public GameObject characterSelectionPanel, playModePanel;
    public GameObject singlePlayerPanel, multiPlayerPanel, hostPanel, joinPanel;

    public SteamManager steamManager;
    public WorldManager worldManager;

    private void OnEnable() {
        characterSelectionPanel.SetActive(true);
        playModePanel.SetActive(false);
    }

    private void Update() {
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
        singlePlayerPanel.SetActive(true);
        multiPlayerPanel.SetActive(false);
    }

    public void OnMultiplayerButtonPressed() {
        singlePlayerPanel.SetActive(false);
        multiPlayerPanel.SetActive(true);
        OnHostPanelButtonPressed();
    }

    public void OnHostPanelButtonPressed() {
        hostPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void OnJoinPanelButtonPressed() {
        joinPanel.SetActive(true);
        hostPanel.SetActive(false);
    }

    public void OnStartSinglePlayerButtonPressed() {
        worldManager.StartSinglePlayerWorld();
    }

    public void OnStartServerHostButtonPressed() {
        steamManager.CreateSteamSocketServer();
    }

    public void OnJoinFriendButtonPressed() {
        steamManager.JoinSteamSocketServer();
    }
}
