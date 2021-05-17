using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    public static ServerDataManager instance;
    private SteamSocketManager _steamSocketManager;
    private readonly byte[] _dataTypeCheck = new byte[1];
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

    public void ProcessRecievedData(IntPtr dataPtr, int size, uint connectionID)
    {
        Debug.Log("SERVER: Processing new saveable data...\n");
        
        // Checks second byte of the data array, which is the TYPE of message
        _dataTypeCheck[0] = System.Runtime.InteropServices.Marshal.ReadByte(dataPtr, 1);
        switch ( _dataTypeCheck[0] )
        {
            // JOIN: "o" (111 in UTF8-Hex / ASCII): Player joined message
            case 111:
                AddToPlayerDatabase(dataPtr, size, connectionID);
                break;
            
            // LEAVE: "p" Does not exist here, since it is called directly
        }
    }
    
    private void AddToPlayerDatabase(IntPtr dataPtr, int size, uint connectionID)
    {
        var dataArray = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, dataArray, 0, size);
        _playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        _playerName = Encoding.UTF8.GetString(dataArray, 19, dataArray.Length-19);
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
        // ! - saveable, q - for new player, 1 - player data
        var dataString = "!qa" + (char)Players.Count;
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
