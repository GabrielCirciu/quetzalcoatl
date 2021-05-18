using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    public static ServerDataManager instance;
    private SteamSocketManager _steamSocketManager;
    private readonly byte[] _dataTypeCheck = new byte[3];
    public Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    public class Player
    {
        public ulong id;
        public string name;
    }

    private ulong _playerID;
    private string _playerName;

    private void Awake()
    {
        if ( instance == null )
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }
    
    public void ClearPlayerDatabase()
    {
        Debug.Log("SERVER: Clearing Player Database (Dictionary)...\n");
        Players.Clear();
    }

    public void ProcessReceivedSaveData(IntPtr dataPtr, int size, uint connectionID)
    {
        Debug.Log("SERVER: Processing new saveable data...\n");
        
        // Saves the second and third bytes and then checks them
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, _dataTypeCheck, 0, 3);
        switch ( _dataTypeCheck[1] )
        {
            // ASCII: P - Player related data
            case 80:
                // Checks third byte of the data array
                switch (_dataTypeCheck[2])
                {
                    // ASCII: J - Joining data
                    case 74:
                        AddToPlayerDatabase(dataPtr, size, connectionID);
                        break;
                    // Leaving data is sent directly from socket manager, not handled here
                }
                break;
        }
    }
    
    private void AddToPlayerDatabase(IntPtr dataPtr, int size, uint connectionID)
    {
        var dataArray = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, dataArray, 0, size);
        _playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 3, 17));
        _playerName = Encoding.UTF8.GetString(dataArray, 20, dataArray.Length-20);
        var newPlayer = new Player
        {
            id = _playerID,
            name = _playerName
        };
        
        Debug.Log($"SERVER: Adding new player [ ID: {_playerID}, Name: {_playerName} ] to the database...\n");
        Players.Add(connectionID, newPlayer);
        
        ShowNewPlayerList();
        SendDataToNewPlayer(connectionID);
    }

    public void ShowNewPlayerList()
    {
        var playerListString = Players.Aggregate("SERVER: New Player list:\n",
            (current, player) =>
                current + "ID: " + Players[player.Key].id + ", Name: " + Players[player.Key].name + "\n");
        Debug.Log(playerListString);
    }

    private void SendDataToNewPlayer(uint connectionID)
    {
        // Grab all player data, pack it up, and send it as 1 package to the new connection
        
        // ASCII: S - Save, N - New client, P - Player data
        var dataString = "SNP" + (char)Players.Count;
        foreach (var player in Players)
        {
            var playerNameLength = (char)Players[player.Key].name.Length;
            var playerName = Players[player.Key].name;
            var playerID = Players[player.Key].id;
            dataString += playerNameLength + playerName + playerID;
        }
        SteamManager.instance.SendDataToNewPlayer(connectionID, dataString);
        
        Debug.Log($"SERVER: Sending all saved data to {Players[connectionID].name}...\nPlayer data: {dataString}\n");
    }
}
