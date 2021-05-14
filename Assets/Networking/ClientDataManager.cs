using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    public static ClientDataManager instance;
    private ChatManager _chatManager;
    private SteamManager _steamManager;

    private Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    private class Player
    {
        public uint connectionID;
        public string name;
    }
    private string _playerName;

    private void Awake()
    {
        instance = this;
    }

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
        const string messageIdentifier = "!o";
        var messageID = SteamClient.SteamId.ToString();
        var idLength = (char) messageID.Length;
        var messageName = SteamClient.Name;
        var messageString = messageIdentifier + idLength + messageID + messageName;
        var messageToByte = Encoding.UTF8.GetBytes(messageString);
        _chatManager.ReceiveJoinedMessage(messageToByte);
        _steamManager.SendMessageToSocketServer(messageToByte);
    }

    public void ProcessRecievedData(byte[] dataArray)
    {
        // Checks second byte of the data array
        switch ( dataArray[1] )
        {
            // CHAT MESSAGE: "n" (110 in UTF8-Hex): Receive chat message
            case 110:
                _chatManager.ReceiveChatMessage(dataArray);
                break;
            
            // CHAT MESSAGE: "o" (111 in UTF8-Hex): Receive joined message
            case 111:
                _chatManager.ReceiveJoinedMessage(dataArray);
                //AddToPlayerDatabase(dataArray);
                break;
        }
    }
    
    /*private void AddToPlayerDatabase(byte[] dataArray)
    {
        Debug.Log("CLIENT: Adding a new player to the database...");

        _playerName = Encoding.UTF8.GetString(dataArray, 2, dataArray.Length-2);
        var newPlayer = new Player
        {
            connectionID = connectionID,
            name = _playerName
        };
        Players.Add(connectionID, newPlayer);
    }*/
}
