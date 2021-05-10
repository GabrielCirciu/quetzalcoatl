using UnityEngine;

public class ChangelogMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;

    private void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            mainMenuManager.OnChangelogButtonPressed();
        }
    }
}
