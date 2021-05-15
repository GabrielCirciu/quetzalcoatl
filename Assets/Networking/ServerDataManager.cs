using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    public static ServerDataManager instance;
    private SteamSocketManager _steamSocketManager;
    private readonly byte[] _dataTypeCheck = new byte[1];
    private Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    private class Player
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
        Debug.Log("SERVER: Clearing Player Database (Dictionary)...");
        Players.Clear();
    }

    public void ProcessRecievedData(IntPtr dataPtr, int size, uint connectionID)
    {
        Debug.Log("SERVER: Processing new saveable data...");
        // Checks second byte of the data array
        _dataTypeCheck[0] = System.Runtime.InteropServices.Marshal.ReadByte(dataPtr, 1);
        switch ( _dataTypeCheck[0] )
        {
            // BYTE: "o" (111 in UTF8-Hex / ASCII): Player joined message
            case 111:
                AddToPlayerDatabase(dataPtr, size, connectionID);
                break;
        }
    }
    
    private void AddToPlayerDatabase(IntPtr dataPtr, int size, uint connectionID)
    {
        Debug.Log("SERVER: Adding a new player to the database...");
        
        var dataArray = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, dataArray, 0, size);
        _playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        _playerName = Encoding.UTF8.GetString(dataArray, 19, dataArray.Length-19);
        var newPlayer = new Player
        {
            id = _playerID,
            name = _playerName
        };
        Players.Add(connectionID, newPlayer);
        
        ShowNewPlayerList();
    }

    public void RemoveFromPlayerDatabase(uint connectionID)
    {
        Debug.Log($"SERVER: Removing player [ ID: {Players[connectionID].id}, Name: {Players[connectionID].name} ] from database...");
        Players.Remove(connectionID);

        ShowNewPlayerList();
    }
    
    private void ShowNewPlayerList()
    {
        var playerListString = "SERVER: New Player list:\n";
        foreach (var player in Players)
            playerListString += "ID: " + Players[player.Key].id + ", Name: " + Players[player.Key].name + "\n";
        Debug.Log(playerListString);
    }
}
