using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;

public class CurrentPlayerList : MonoBehaviour {
    public GameObject playerNameObj, contentPanel, currentPlayerListCanvas;
    private readonly List<Player> _playerList = new List<Player>();
    private class Player {
        public int namelength, overflow;
        public string name;
        public TMP_Text textText;
    }

    private SteamManager _steamManager;

    private void Start() {
        AddSelfToCurrentPlayerList();
        _steamManager = GameObject.Find("SteamManager").GetComponent<SteamManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!currentPlayerListCanvas.activeSelf)
            {
                currentPlayerListCanvas.SetActive(true);
                UpdateCurrentPlayerList();
            }
            else currentPlayerListCanvas.SetActive(false);
        }
    }

    private void UpdateCurrentPlayerList()
    {
        Debug.Log(_steamManager.steamSocketManager.Connected[0].Id.ToString());
        // Get server player data and display them
    }

    private void AddSelfToCurrentPlayerList() {
        var newPlayer = new Player();
        newPlayer.name = SteamClient.Name;
        var newTextObject = Instantiate(playerNameObj, contentPanel.transform);
        newPlayer.textText = newTextObject.GetComponent<TMP_Text>();
        newPlayer.textText.text = newPlayer.name;
        _playerList.Add(newPlayer);
    }

    public void AddToCurrentPlayerList(byte[] dataArray) {
        var newPlayer = new Player();
        newPlayer.overflow = int.Parse(System.Text.Encoding.UTF8.GetString(dataArray, 1, 1));
        newPlayer.namelength = int.Parse(System.Text.Encoding.UTF8.GetString(dataArray, 2, 1+newPlayer.overflow));
        newPlayer.name = System.Text.Encoding.UTF8.GetString(dataArray, 8+newPlayer.overflow, newPlayer.namelength);
        var newTextObject = Instantiate(playerNameObj, contentPanel.transform);
        newPlayer.textText = newTextObject.GetComponent<TMP_Text>();
        newPlayer.textText.text = newPlayer.name;
        _playerList.Add(newPlayer);
    }

    public void RemoveFromCurrentPlayerList() {
        // Will have to add a method to check through all players and then remove the one who left the server
        Destroy(_playerList[0].textText.gameObject);
        _playerList.Remove(_playerList[0]);
    }
}
