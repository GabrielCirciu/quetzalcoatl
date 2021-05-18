using System.Text;
using Steamworks;
using UnityEngine;

public class CharacterNetworkedStats : MonoBehaviour
{
    public static CharacterNetworkedStats instance;
    private SteamManager _steamManager;
    private ClientDataManager _clientDataManager;
    private ulong _localSteamID, _receivedSteamID;
    private string _dataString, _receivedDataString;
    private Transform _localPlayerTransform;
    private Vector3 _localPlayerVector3, _receivedPlayerVector3;
    
    // ASCII: N - No save, P - Player, P - Position (send ID, position, rotation, etc.)
    private const string DataIdentifier = "NPP";

    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _clientDataManager = ClientDataManager.instance;
        _localSteamID = SteamClient.SteamId.Value;
        
        _localPlayerTransform = GetComponent<Transform>();

        InvokeRepeating(nameof(SendNetworkedCharacterData), 5.0f, 5.0f);
    }

    private void SendNetworkedCharacterData()
    {
        _localPlayerVector3 = _localPlayerTransform.position;
        _dataString = DataIdentifier + _localSteamID + _localPlayerVector3.x.ToString("+0000.00;-0000.00") +
                      _localPlayerVector3.y.ToString("+0000.00;-0000.00") + _localPlayerVector3.z.ToString("+0000.00;-0000.00");
        var dataArray = Encoding.UTF8.GetBytes(_dataString);
        _steamManager.SendMessageToSocketServer(dataArray);
    }

    public void ReceiveNetworkedCharacterData(byte[] dataArray)
    {
        // Change a character position based on ID.
        _receivedSteamID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 3, 17));
        _receivedDataString = Encoding.UTF8.GetString(dataArray, 20, dataArray.Length - 20);
        //_receivedPlayerVector3 = _clientDataManager.players[_receivedSteamID].character.transform.position;
        _receivedPlayerVector3.x = float.Parse(Encoding.UTF8.GetString(dataArray, 20, 8));
        //_receivedPlayerVector3.y = int.Parse(Encoding.UTF8.GetString(dataArray, 28, 8));
        //_receivedPlayerVector3.z = int.Parse(Encoding.UTF8.GetString(dataArray, 36, 8));
        
        Debug.Log($"CLIENT: Received character data...\nID: {_receivedSteamID}, Pos: {_receivedDataString}\n");
    }
}
