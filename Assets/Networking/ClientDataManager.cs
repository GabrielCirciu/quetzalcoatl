using UnityEngine;
using Steamworks;
using System.Text;

public class ClientDataManager : MonoBehaviour
{
    public static ClientDataManager instance;
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
        _steamManager.EnableClientDataManager();
        OnJoinedServer();
    }
    
    private void OnJoinedServer()
    {
        // ASCII: ! - Save on server, o - Join Message
        const string messageIdentifier = "!o";
        var messageName = SteamClient.Name;
        var encodedMessage = messageIdentifier + messageName;
        var messageToByte = Encoding.UTF8.GetBytes(encodedMessage);
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
                break;
        }
    }
}
