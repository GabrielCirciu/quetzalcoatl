using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    public SteamManager steamManager;
    public ChatManager chatManager;

    void Start() {
        steamManager = GameObject.Find("SteamManager").GetComponent<SteamManager>();
        steamManager.ActivateDataManager();
    }

    public void ProcessRecievedData(byte[] dataArray) {
        // Checks first byte of the data array
        Debug.Log("Recieved a message and processing it");
        switch ( dataArray[0] ) {
            // CHAT MESSAGE: "n" (110 in UTF8-Hex): Send chat message
            case 110:
                chatManager.RecieveChatMessage(dataArray);
                break;
            // PLAYER JOINED: "o" (111 in UTF8-Hex): Send player joined chat message
            case 111:
                chatManager.JoinedChatMessage(dataArray);
                break;
        }
    }
}
