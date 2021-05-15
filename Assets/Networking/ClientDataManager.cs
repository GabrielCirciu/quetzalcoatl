using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    public static ClientDataManager instance;
    private ChatManager _chatManager;
    private SteamManager _steamManager;

    private ulong _playerID;
    private string _playerName;
    private readonly Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();
    private class Player
    {
        public ulong id;
        public string name;
    }

    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _chatManager = ChatManager.instance;
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
            // CHAT: "n" (in ASCII): Receive chat message
            case 110:
                _chatManager.ReceiveChatMessage(dataArray);
                break;
            
            // JOIN: "o" (in ASCII): Receive join message
            case 111:
                _chatManager.ReceiveJoinMessage(dataArray);
                AddToPlayerDatabase(dataArray);
                break;
            
            // LEAVE: "p" (in ASCII): Receive leave message
            case 112:
                RemoveFromPlayerDatabase(dataArray);
                break;
        }
    }
    
    private void AddToPlayerDatabase(byte[] dataArray)
    {
        Debug.Log("CLIENT: Adding a new player to the database...");
        _playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        _playerName = Encoding.UTF8.GetString(dataArray, 19, dataArray.Length-19);
        var newPlayer = new Player
        {
            id = _playerID,
            name = _playerName
        };
        _players.Add(_playerID, newPlayer);
    }

    private void RemoveFromPlayerDatabase(byte[] dataArray)
    {
        Debug.Log("CLIENT: Removing a player from the database...");
        _playerID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 2, 17));
        _players.Remove(_playerID);
    }
}
