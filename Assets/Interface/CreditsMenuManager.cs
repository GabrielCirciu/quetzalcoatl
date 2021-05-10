using UnityEngine;

public class CreditsMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;

    private void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            mainMenuManager.OnCreditsButtonPressed();
        }
    }
}
