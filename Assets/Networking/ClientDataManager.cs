using System;
using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    private ClientDataManager instance;
    private ChatManager _chatManager;
    private SteamManager _steamManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _chatManager = ChatManager.instance;
        OnJoinedServer();
    }
    
    private void OnJoinedServer()
    {
        // ASCII: ! - Save on server, o - Join Message
        const string messageIdentifier = "!o";
        var messageName = SteamClient.Name;
        var encodedMessage = messageIdentifier + messageName;
        var messageToByte = Encoding.UTF8.GetBytes(encodedMessage);
        _steamManager.SendMessageToSocketServer(messageToByte);
        _chatManager.ReceiveJoinedMessage(messageToByte);
    }

    public void ProcessRecievedData(byte[] dataArray)
    {
        // Checks second byte of the data array
        Debug.Log("CLIENT: Processing recieved data");
        switch ( dataArray[1] )
        {
            // CHAT MESSAGE: "n" (110 in UTF8-Hex): Send chat message
            case 110:
                _chatManager.ReceiveChatMessage(dataArray);
                break;
            
            // PLAYER JOINED: "o" (111 in UTF8-Hex): Send player joined chat message
            case 111:
                _chatManager.ReceiveJoinedMessage(dataArray);
                break;
        }
    }
}
