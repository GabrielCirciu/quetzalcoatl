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
        public string id, name;
    }
    private string _playerID, _playerName;

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
        var pIDSize = dataArray[2];
        _playerID = Encoding.UTF8.GetString(dataArray, 3, pIDSize);
        _playerName = Encoding.UTF8.GetString(dataArray, 3+pIDSize, dataArray.Length-3-pIDSize);
        var newPlayer = new Player
        {
            id = _playerID,
            name = _playerName
        };
        Players.Add(connectionID, newPlayer);

        UpdateClientDatabse();
    }

    public void RemoveFromPlayerDatabase(uint connectionID)
    {
        Debug.Log($"SERVER: Removing player [ {Players[connectionID].name} ] from database...");
        Players.Remove(connectionID);

        UpdateClientDatabse();
    }
    
    private void UpdateClientDatabse()
    {
        Debug.Log("SERVER: New player list:");
        foreach (var player in Players)
            Debug.Log($"Player [ ID: {Players[player.Key].id}, Name: {Players[player.Key].name} ]");
    }
}
