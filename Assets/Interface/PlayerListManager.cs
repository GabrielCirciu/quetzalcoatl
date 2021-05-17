using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListManager : MonoBehaviour
{
    public static PlayerListManager instance;
    public GameObject playerNameObj, contentPanel, currentPlayerListCanvas;
    private readonly List<Player> _playerList = new List<Player>();
    private class Player
    {
        public ulong id;
        public TMP_Text text;
    }

    private void Awake() => instance = this;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;
        currentPlayerListCanvas.SetActive(!currentPlayerListCanvas.activeSelf);
    }

    public void AddToPlayerList(ulong playerID, string playerName)
    {
        var newPlayerObj = Instantiate(playerNameObj, contentPanel.transform);
        var newPlayer = new Player
        {
            id = playerID,
            text = newPlayerObj.GetComponent<TMP_Text>()
        };
        _playerList.Add(newPlayer);
        newPlayer.text.text = playerName;
    }

    public void RemoveFromPlayerList(ulong playerID)
    {
        for (var i = 0; i < _playerList.Count; i++)
        {
            if (_playerList[i].id != playerID) continue;
            Destroy(_playerList[i].text.gameObject);
            _playerList.Remove(_playerList[i]);
            break;
        }
    }
}
