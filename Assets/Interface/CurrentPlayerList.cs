using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentPlayerList : MonoBehaviour {
    public GameObject playerNameObj, contentPanel, currentPlayerListCanvas;
    private readonly List<Player> _playerList = new List<Player>();
    private class Player {
        public int namelength, overflow;
        public string name;
        public TMP_Text textText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!currentPlayerListCanvas.activeSelf)
            {
                currentPlayerListCanvas.SetActive(true);
            }
            else currentPlayerListCanvas.SetActive(false);
        }
    }
}
