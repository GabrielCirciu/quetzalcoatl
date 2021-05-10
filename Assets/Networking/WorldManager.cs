using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;
using System;

public class WorldManager : MonoBehaviour {
    private static WorldManager _instance;
    public SteamManager steamManager;
    public GameObject chatManagerObj;

    private void Awake(){
        if ( _instance == null ) {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if ( _instance != this ) { Destroy(gameObject); }
    }

    public void StartSinglePlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
    }

    public void StartMultiPlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
    }

    public void JoinMultiPlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
        const string messageIdentifier = "o";
        var messageTimeStamp = DateTime.Now.ToString("HH:mm");
        var messageName = SteamClient.Name;
        var messageNameLength = messageName.Length;
        var messageNameOverflow = 0;
        if ( messageNameLength > 9 ) messageNameOverflow = 1;
        const string joinedText = " has joined the world!";
        var encodedMessage = messageIdentifier + messageNameOverflow + messageNameLength + messageTimeStamp +
                             messageName + joinedText;
        var messageToByte = System.Text.Encoding.UTF8.GetBytes(encodedMessage);
        steamManager.SendMessageToSocketServer(messageToByte);
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("SampleScene");
    }
}
