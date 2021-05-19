using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.Runtime.InteropServices.Marshal;

public class ServerDataManager : MonoBehaviour
{
    // ASCII: S - Save, N - New client, P - Player data
    private const string NewClientDataIdentifier = "SNP";
    private readonly byte[] _dataTypeCheck = new byte[3];
    
    public static ServerDataManager instance;

    private ulong _playerID;
    private string _playerName;
    private byte[] _newClientDataIdArray, _finalDataArray;

    public readonly Dictionary<uint, Player> players = new Dictionary<uint, Player>();
    public class Player
    {
        public ulong id;
        public string name;
    }

    private void Awake()
    {
        if ( instance == null )
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        _newClientDataIdArray = Encoding.UTF8.GetBytes(NewClientDataIdentifier);
    }

    public void ClearPlayerDatabase()
    {
        Debug.Log("SERVER: Clearing Player Database (Dictionary)...\n");
        players.Clear();
    }

    public void ProcessReceivedSaveData(IntPtr dataPtr, int size, uint connectionId)
    {
        Debug.Log("SERVER: Processing new saveable data...\n");
        
        // Saves the first three bytes and then checks them starting from second position
        Copy(dataPtr, _dataTypeCheck, 0, 3);
        switch ( _dataTypeCheck[1] )
        {
            case 80: // P - Player related data
                switch (_dataTypeCheck[2]) // Checks third byte of the data array
                {
                    case 74: // J - Joining data
                        AddToPlayerDatabase(dataPtr, size, connectionId);
                        break;
                    
                    // Player leaving data is sent directly from socket manager, not handled here
                }
                break;
        }
    }
    
    private void AddToPlayerDatabase(IntPtr dataPtr, int size, uint connectionId)
    {
        var dataArray = new byte[size];
        Copy(dataPtr, dataArray, 0, size);
        
        _playerID = BitConverter.ToUInt64(dataArray, 3);
        _playerName = Encoding.UTF8.GetString(dataArray, 11, dataArray.Length-11);
        var newPlayer = new Player
        {
            id = _playerID,
            name = _playerName
        };
        players.Add(connectionId, newPlayer);
        
        ShowNewPlayerList();
        SendSavedDataToNewPlayer(connectionId);
        
        Debug.Log($"SERVER: Adding new player [ ID: {_playerID}, Name: {_playerName} ] to the database...\n");
    }

    public void ShowNewPlayerList()
    {
        var playerListString = players.Aggregate("SERVER: New Player list:\n",
            (current, player) =>
                current + "ID: " + players[player.Key].id + ", Name: " + players[player.Key].name + "\n");
        Debug.Log(playerListString);
    }

    private void SendSavedDataToNewPlayer(uint connectionId)
    {
        // Grab all player data, pack it up, and send it as 1 package to the new connection
        var playerNamesLength = players.Sum(player => players[player.Key].name.Length);
        _finalDataArray = new byte[3 + 1 + 8 * players.Count + playerNamesLength];
        Buffer.BlockCopy(_newClientDataIdArray, 0, _finalDataArray, 0, 3);
        
        var arrayOffset = 3;
        _finalDataArray[arrayOffset] = Convert.ToByte(players.Count);
        arrayOffset++;
        foreach (var player in players)
        {
            var steamIdArray = BitConverter.GetBytes(players[player.Key].id);
            Buffer.BlockCopy(steamIdArray, 0, _finalDataArray, arrayOffset, 8);
            arrayOffset += 8;
            _finalDataArray[arrayOffset] = Convert.ToByte(players[player.Key].name.Length);
            arrayOffset++;
            var nameArray = Encoding.UTF8.GetBytes(players[player.Key].name);
            Buffer.BlockCopy(nameArray, 0, _finalDataArray, arrayOffset, players[player.Key].name.Length);
            arrayOffset += players[player.Key].name.Length;
        }
        
        SteamManager.instance.SendDataToNewPlayer(connectionId, _finalDataArray);
        Debug.Log($"SERVER: Sending all saved data to {players[connectionId].name}...\nPlayer data: {_finalDataArray}\n");
    }
}
