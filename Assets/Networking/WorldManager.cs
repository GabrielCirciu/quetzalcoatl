using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour {
    public static WorldManager Instance;

    void Awake(){
        if ( Instance == null ) {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else if ( Instance != this ) { Destroy(gameObject); }
    }

    public void StartSinglePlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
    }

    public void StartMultiPlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("SampleScene");
    }
}
