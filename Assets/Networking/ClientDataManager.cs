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
        _steamManager.EnableClientDataManager();
        OnJoinedServer();
    }
    
    private void OnJoinedServer()
    {
        // ASCII: S - Save, P - Player, J - Join
        // Accound ID is a Steam 64 ID, which is 17 digits long
        const string messageIdentifier = "SPJ";
        var accountID = SteamClient.SteamId.Value;
        var accountName = SteamClient.Name;
        var messageString = messageIdentifier + accountID + accountName;
        var messageToByte = Encoding.UTF8.GetBytes(messageString);
        _steamManager.SendMessageToSocketServer(messageToByte);
        _chatManager.ReceiveJoinOrLeaveMessage(accountName, " has joined the world!");
    }

    public void ProcessRecievedData(byte[] dataArray)
    {
        // Checks second byte of the data array
        switch ( dataArray[1] )
        {
            // ASCII: P - Player
            case 80:
                // Checks third byte of the data array
                switch ( dataArray[2] )
                {
                    // ASCII: P - Position/Rotation/Velocity data
                    case 80:
                        _characterNetworkedStats.ReceiveNetworkedCharacterData(dataArray);
                        break;
                    // ASCII: J - Joined server
                    case 74:
                        AddToPlayerDatabase(dataArray);
                        break;
                    // ASCII: L - Left server
                    case 112:
                        RemoveFromPlayerDatabase(dataArray);
                        break;
                }
                break;

            // ASCII: C - Chat
            case 67:
                // Checks third byte of the data array
                switch ( dataArray[2] )
                {
                    // ASCII: G - General chat message
                    case 71:
                        _chatManager.ReceiveChatMessage(dataArray);
                        break;
                }
                break;

            // ASCII: N - New player data received on joining server
            case 78:
                // Checks third byte of the data array
                switch ( dataArray[2] )
                {
                    // ASCII: P - Player data
                    case 80:
                        ProcessAllPlayerData(dataArray);
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
