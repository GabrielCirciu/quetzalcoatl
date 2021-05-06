using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangelogMenuManager : MonoBehaviour {
    public MainMenuManager mainMenuManager;

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            mainMenuManager.OnChangelogButtonPressed();
        }
    }
}
