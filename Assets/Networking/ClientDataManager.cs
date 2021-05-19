using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    // ASCII: S - Save, P - Player, J - Join
    private const string JoinDataIdentifier = "SPJ";
    
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
        _joinDataIdArray = Encoding.UTF8.GetBytes(JoinDataIdentifier);
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
                    case 80: // P - Position/Rotation/Velocity data
                        _characterNetworkedStats.ReceiveNetworkedCharacterData(dataArray);
                        break;
                    case 74: // J - Joined server
                        //AddToPlayerDatabase(dataArray);
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
                        //ProcessAllPlayerData(dataArray);
                        break;
                }
                break;
        }
    }

    private void ProcessAllPlayerData(byte[] dataArray)
    {
        var dataString = Encoding.UTF8.GetString(dataArray, 0, dataArray.Length);
        Debug.Log($"CLIENT: Received server-side player save data array:\n{dataString}\n");

        var playerAmount = dataArray[3];
        var arrayIndex = 4;
        var mySteamID = SteamClient.SteamId;
        
        for (var i = 0; i < playerAmount; i++)
        {
            var playerNameLength = dataArray[arrayIndex];
            var playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, arrayIndex+playerNameLength+1, 17));
            var playerName = Encoding.UTF8.GetString(dataArray, arrayIndex+1, playerNameLength);
            var newPlayer = new Player
            {
                id = playerID,
                name = playerName
            };
            players.Add(playerID, newPlayer);
            arrayIndex += playerNameLength + 17 + 1;
            Debug.Log($"CLIENT: Added new player [ ID: {players[playerID].id}, Name: {players[playerID].name} ] to database...\n");
            
            _playerListManager.AddToPlayerList(playerID, playerName);

            if (players[playerID].id != mySteamID) SpawnCharacter(playerID);
        }
    }
    
    private void AddToPlayerDatabase(byte[] dataArray)
    {
        var playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 3, 17));
        var playerName = Encoding.UTF8.GetString(dataArray, 20, dataArray.Length-20);
        var newPlayer = new Player
        {
            id = playerID,
            name = playerName
        };
        players.Add(playerID, newPlayer);
        Debug.Log($"CLIENT: Added new player [ ID: {players[playerID].id}, Name: {players[playerID].name} ] to database...\n");
        
        _playerListManager.AddToPlayerList(playerID, playerName);
        _chatManager.ReceiveJoinOrLeaveMessage(playerName, " has joined the world!");
        SpawnCharacter(playerID);
    }

    private void SpawnCharacter(ulong playerID)
    {
        players[playerID].character = Instantiate(characterObj, characterSpawner.transform);
    }

    private void RemoveFromPlayerDatabase(byte[] dataArray)
    {
        var playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 3, 17));
        Debug.Log($"CLIENT: Removing [ ID: {players[playerID].id}, Name: {players[playerID].name} ] from the database...\n");
        
        _chatManager.ReceiveJoinOrLeaveMessage(players[playerID].name, " has left the world!");
        Destroy(players[playerID].character);
        players.Remove(playerID);
        _playerListManager.RemoveFromPlayerList(playerID);
    }
}
