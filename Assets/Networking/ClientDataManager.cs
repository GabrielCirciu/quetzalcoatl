using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    // ASCII: S - Save, P - Player, J - Join
    private const string JoinDataId = "SPJ";
    
    public static ClientDataManager instance;
    private ChatManager _chatManager;
    private SteamManager _steamManager;
    private PlayerListManager _playerListManager;
    private CharacterNetworkedStats _characterNetworkedStats;

    [SerializeField] private GameObject characterObj, characterSpawner;

    private string _localPlayerName;
    private byte[] _joinDataIdArray, _localSteamIdArray, _nameArray, _finalDataArray;

    public readonly Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();
    public class Player
    {
        public ulong id;
        public string name;
        public GameObject character;
    }

    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _chatManager = ChatManager.instance;
        _playerListManager = PlayerListManager.instance;
        _characterNetworkedStats = CharacterNetworkedStats.instance;
        
        _localPlayerName = SteamClient.Name;
        _joinDataIdArray = Encoding.UTF8.GetBytes(JoinDataId);
        _localSteamIdArray = BitConverter.GetBytes(SteamClient.SteamId.Value);
        _nameArray = Encoding.UTF8.GetBytes(_localPlayerName);
        
        _steamManager.EnableClientDataManager();
        OnJoinedServer();
    }
    
    private void OnJoinedServer()
    {
        _finalDataArray = new byte[3 + 8 + _nameArray.Length];
        Buffer.BlockCopy(_joinDataIdArray, 0, _finalDataArray, 0, 3);
        Buffer.BlockCopy(_localSteamIdArray, 0, _finalDataArray, 3, 8);
        Buffer.BlockCopy(_nameArray, 0, _finalDataArray, 11, _nameArray.Length);
        
        _steamManager.SendMessageToSocketServer(_finalDataArray);
        _chatManager.ReceiveJoinOrLeaveMessage(_localPlayerName, " has joined the world!");
    }

    public void ProcessRecievedData(byte[] dataArray)
    {
        switch ( dataArray[1] ) // Checks second byte of the data array
        {
            case 80: // P - Player
                switch ( dataArray[2] ) // Checks third byte of the data array
                {
                    case 84: // T - Transform
                        _characterNetworkedStats.ReceiveNetworkedCharacterData(dataArray);
                        break;
                    case 74: // J - Joined server
                        AddToPlayerDatabase(dataArray);
                        break;
                    case 76: // L - Left server
                        RemoveFromPlayerDatabase(dataArray);
                        break;
                }
                break;
            
            case 67: // C - Chat
                switch ( dataArray[2] ) // Checks third byte of the data array
                {
                    case 71: // G - General chat message
                        _chatManager.ReceiveChatMessage(dataArray);
                        break;
                }
                break;
            
            case 78: // N - New player data received on joining server
                switch ( dataArray[2] ) // Checks third byte of the data array
                {
                    case 80: // P - Player data
                        ProcessAllPlayerData(dataArray);
                        break;
                }
                break;
        }
    }

    private void ProcessAllPlayerData(byte[] dataArray)
    {
        Debug.Log($"CLIENT: Received server-side player save data array:\n");
        var arrayOffset = 3;
        var playerAmount = dataArray[arrayOffset];
        arrayOffset++;
        var mySteamID = SteamClient.SteamId;
        
        for (var i = 0; i < playerAmount; i++)
        {
            var playerId = BitConverter.ToUInt64(dataArray, arrayOffset);
            arrayOffset += 8;
            var playerNameLength = dataArray[arrayOffset];
            arrayOffset++;
            var playerName = Encoding.UTF8.GetString(dataArray, arrayOffset, playerNameLength);
            var newPlayer = new Player
            {
                id = playerId,
                name = playerName
            };
            players.Add(playerId, newPlayer);
            arrayOffset += playerNameLength;

            _playerListManager.AddToPlayerList(playerId, playerName);
            if (players[playerId].id != mySteamID) SpawnCharacter(playerId);
            Debug.Log($"CLIENT: Added new player [ ID: {players[playerId].id}, Name: {players[playerId].name} ] to database...\n");
        }
    }
    
    private void AddToPlayerDatabase(byte[] dataArray)
    {
        var playerId = BitConverter.ToUInt64(dataArray, 3);
        var playerName = Encoding.UTF8.GetString(dataArray, 11, dataArray.Length-11);
        var newPlayer = new Player
        {
            id = playerId,
            name = playerName
        };
        players.Add(playerId, newPlayer);
        
        _playerListManager.AddToPlayerList(playerId, playerName);
        _chatManager.ReceiveJoinOrLeaveMessage(playerName, " has joined the world!");
        SpawnCharacter(playerId);
        Debug.Log($"CLIENT: Added new player [ ID: {players[playerId].id}, Name: {players[playerId].name} ] to database...\n");
    }

    // More things will be handled here, like all customization initialization
    private void SpawnCharacter(ulong playerID)
    {
        players[playerID].character = Instantiate(characterObj, characterSpawner.transform);
    }

    private void RemoveFromPlayerDatabase(byte[] dataArray)
    {
        var playerID = BitConverter.ToUInt64(dataArray, 3);
        Debug.Log($"CLIENT: Removing [ ID: {players[playerID].id}, Name: {players[playerID].name} ] from the database...\n");
        
        _chatManager.ReceiveJoinOrLeaveMessage(players[playerID].name, " has left the world!");
        Destroy(players[playerID].character);
        _playerListManager.RemoveFromPlayerList(playerID);
        players.Remove(playerID);
    }
}
