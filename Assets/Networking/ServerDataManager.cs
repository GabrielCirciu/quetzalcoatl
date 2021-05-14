using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    public static ServerDataManager instance;
    private Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    private class Player
    {
        public uint connectionID;
        public string name;
    }
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

    public void ProcessRecievedData(IntPtr dataPtr, int size, uint connectionID)
    {
        Debug.Log("SERVER: Processing save data");
        var dataArray = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, dataArray, 0, size);
        
        // Checks second byte of the data array
        switch ( dataArray[1] )
        {
            // BYTE: "o" (111 in UTF8-Hex / ASCII): Player joined message
            case 111:
                AddToPlayerData(dataArray, connectionID);
                break;
        }
    }

    public void RemoveFromPlayerDatabase(uint connectionID)
    {
        Debug.Log($"SERVER: Removing player [ {Players[connectionID].name} ] from database...");
        Players.Remove(connectionID);
        foreach (var player in Players)
            Debug.Log($"Players [ ID: {Players[player.Key].connectionID}, Name: {Players[player.Key].name} ]");
    }

    private void AddToPlayerData(byte[] dataArray, uint connectionID)
    {
        Debug.Log("SERVER: Adding a new player to the database...");
        _playerName = Encoding.UTF8.GetString(dataArray, 2, dataArray.Length-2);
        var newPlayer = new Player
        {
            connectionID = connectionID,
            name = _playerName
        };
        Players.Add(connectionID, newPlayer);
        foreach (var player in Players)
            Debug.Log($"Player [ ID: {Players[player.Key].connectionID}, Name: {Players[player.Key].name} ]");
    }

    public void ClearPlayerDatabase()
    {
        Debug.Log("Clearing Player Database (Dictionary)...");
        Players.Clear();
    }
}
