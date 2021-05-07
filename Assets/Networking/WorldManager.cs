using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;
using System;

public class WorldManager : MonoBehaviour {
    public static WorldManager Instance;
    public SteamManager steamManager;
    public GameObject chatManagerObj;

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

    public void JoinMultiPlayerWorld() {
        SceneManager.LoadScene("CharacterAnimations");
        string messageIdentifier = "o";
        string messageTimeStamp = DateTime.Now.ToString("HH:mm");
        string messageName = SteamClient.Name.ToString();
        int messageNameLength = messageName.Length;
        int messageNameOverflow = 0;
        if (messageNameLength > 9) messageNameOverflow = 1;
        string joinedText = " has joined the world!";
        string encodedMessage = messageIdentifier+messageNameOverflow+messageNameLength.ToString()+messageTimeStamp+messageName+joinedText;
        byte[] messageToByte = System.Text.Encoding.UTF8.GetBytes(encodedMessage);
        steamManager.SendMessageToSocketServer(messageToByte);
        Debug.Log("We start looking for the Chat Manager");
        chatManagerObj = GameObject.Find("ChatManager");
        Debug.Log("We found the Chat Manager Object");
        ChatManager chatManager = chatManagerObj.GetComponent<ChatManager>();
        Debug.Log("We found the Chat Manager Component");
        chatManager.JoinedChatMessage(messageToByte);
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("SampleScene");
    }
}
