using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    public SteamManager steamManager;
    public ChatManager chatManager;

    void Start() {
        steamManager = GameObject.Find("SteamManagerGameObject").GetComponent<SteamManager>();
        steamManager.ActivateDataManager();
    }

    public void ProcessRecievedData(byte[] dataArray) {
        // Checks first byte if it's "n" which is 110 in UTF8-Hex encoding
        if (dataArray[0] == 110) {
            chatManager.RecieveChatMessage(dataArray);
        }
    }
}
