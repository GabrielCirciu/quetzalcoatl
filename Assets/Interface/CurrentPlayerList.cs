using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;

public class CurrentPlayerList : MonoBehaviour {
    public GameObject playerNameObj, contentPanel, currentPlayerListCanvas;
    public List<Player> playerList = new List<Player>();
    public class Player {
        public int namelength, overflow;
        public string name;
        public TMP_Text textText;
    }

    void Start() {
        AddSelfToCurrentPlayerList();
    }

    void Update() {
        if ( Input.GetKeyDown(KeyCode.Tab) ) currentPlayerListCanvas.SetActive(!currentPlayerListCanvas.activeSelf);
    }

    void AddSelfToCurrentPlayerList() {
        Player newPlayer = new Player();
        newPlayer.name = SteamClient.Name.ToString();
        GameObject newTextObject = Instantiate(playerNameObj, contentPanel.transform);
        newPlayer.textText = newTextObject.GetComponent<TMP_Text>();
        newPlayer.textText.text = newPlayer.name;
        playerList.Add(newPlayer);
    }

    public void AddToCurrentPlayerList(byte[] dataArray) {
        Player newPlayer = new Player();
        newPlayer.overflow = int.Parse(System.Text.Encoding.UTF8.GetString(dataArray, 1, 1));
        newPlayer.namelength = int.Parse(System.Text.Encoding.UTF8.GetString(dataArray, 2, 1+newPlayer.overflow));
        newPlayer.name = System.Text.Encoding.UTF8.GetString(dataArray, 8+newPlayer.overflow, newPlayer.namelength);
        int textStartPos = 8 + newPlayer.overflow + newPlayer.namelength;
        GameObject newTextObject = Instantiate(playerNameObj, contentPanel.transform);
        newPlayer.textText = newTextObject.GetComponent<TMP_Text>();
        newPlayer.textText.text = newPlayer.name;
        playerList.Add(newPlayer);
    }

    public void RemoveFromCurrentPlayerList() {
        // Will have to add a method to check through all players and then remove the one who left the server
    }
}
