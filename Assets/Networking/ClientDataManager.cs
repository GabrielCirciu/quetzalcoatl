using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    public static ClientDataManager instance;
    private ChatManager _chatManager;
    private SteamManager _steamManager;
    private PlayerListManager _playerListManager;
    private CharacterNetworkedStats _characterNetworkedStats;

    [SerializeField] private GameObject characterObj, characterSpawner;
    
    private readonly Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();
    private class Player
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
        _steamManager.EnableClientDataManager();
        OnJoinedServer();
    }
    
    private void OnJoinedServer()
    {
        // ASCII: ! - Save on server, o - Join Message
        // Accound ID is a Steam 64 ID, which is 17 digits long
        const string messageIdentifier = "!o";
        var accountID = SteamClient.SteamId.Value;
        var accountName = SteamClient.Name;
        var messageString = messageIdentifier + accountID + accountName;
        var messageToByte = Encoding.UTF8.GetBytes(messageString);
        _steamManager.SendMessageToSocketServer(messageToByte);
        _chatManager.ReceiveJoinMessage(messageToByte);
    }

    public void ProcessRecievedData(byte[] dataArray)
    {
        // Checks second byte of the data array
        switch ( dataArray[1] )
        {
            // CHARACTER: "1" (in ASCII): Receive character data (position/ rotation/ velocity/ etc.)
            case 49:
                _characterNetworkedStats.ReceiveNetworkedCharacterData();
                break;
            
            // CHAT: "n" (in ASCII): Receive chat message
            case 110:
                _chatManager.ReceiveChatMessage(dataArray);
                break;
            
            // JOIN: "o" (in ASCII): Receive join chat message and add new player to database
            case 111:
                _chatManager.ReceiveJoinMessage(dataArray);
                AddToPlayerDatabase(dataArray);
                break;
            
            // LEAVE: "p" (in ASCII): Receive leave data, remove specific player
            case 112:
                RemoveFromPlayerDatabase(dataArray);
                break;
            
            // NEW: "q" (in ASCII): Get all saved data from server as a new player
            case 113:
                ReceiveOnJoinData(dataArray);
                break;
        }
    }

    private void ReceiveOnJoinData(byte[] dataArray)
    {
        // Checks third byte of the data array
        switch ( dataArray[2] )
        {
            // PLAYER DATA: "a" (in ASCII): Received all server-side player data
            case 97:
                ProcessAllPlayerData(dataArray);
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
            _players.Add(playerID, newPlayer);
            arrayIndex += playerNameLength + 17 + 1;
            Debug.Log($"CLIENT: Added new player [ ID: {_players[playerID].id}, Name: {_players[playerID].name} ] to database...\n");
            
            _playerListManager.AddToPlayerList(playerID, playerName);

            if (_players[playerID].id != mySteamID) SpawnCharacter(playerID);
        }
    }
    
    private void AddToPlayerDatabase(byte[] dataArray)
    {
        var playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        var playerName = Encoding.UTF8.GetString(dataArray, 19, dataArray.Length-19);
        var newPlayer = new Player
        {
            id = playerID,
            name = playerName
        };
        _players.Add(playerID, newPlayer);
        Debug.Log($"CLIENT: Added new player [ ID: {_players[playerID].id}, Name: {_players[playerID].name} ] to database...\n");
        
        _playerListManager.AddToPlayerList(playerID, playerName);
        
        SpawnCharacter(playerID);
    }

    private void SpawnCharacter(ulong playerID)
    {
        _players[playerID].character = Instantiate(characterObj, characterSpawner.transform);
    }

    private void RemoveFromPlayerDatabase(byte[] dataArray)
    {
        var playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        Debug.Log($"CLIENT: Removing [ ID: {_players[playerID].id}, Name: {_players[playerID].name} ] from the database...\n");
        
        Destroy(_players[playerID].character);
        _players.Remove(playerID);
        _playerListManager.RemoveFromPlayerList(playerID);
    }
}
