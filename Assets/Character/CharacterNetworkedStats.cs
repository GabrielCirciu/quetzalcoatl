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
    private Vector3 _localPlayerPositionVector, _receivedPlayerPositionVector;
    private float _localPlayerRotationVectorY, _receivedPlayerRotationVectorY;

    // ASCII: N - No save, P - Player, P - Position (send ID, position, rotation, etc.)
    private const string DataIdentifier = "NPP";

    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _clientDataManager = ClientDataManager.instance;
        _localSteamID = SteamClient.SteamId.Value;
        _localPlayerTransform = GetComponent<Transform>();

        InvokeRepeating(nameof(SendNetworkedCharacterData), 3.0f, 0.1f);
    }

    private void SendNetworkedCharacterData()
    {
        _localPlayerPositionVector = _localPlayerTransform.position;
        _localPlayerRotationVectorY = _localPlayerTransform.rotation.eulerAngles.y;

        _dataString = DataIdentifier + _localSteamID +
                      _localPlayerPositionVector.x.ToString("+0000.00;-0000.00") +
                      _localPlayerPositionVector.y.ToString("+0000.00;-0000.00") +
                      _localPlayerPositionVector.z.ToString("+0000.00;-0000.00") +
                      _localPlayerRotationVectorY.ToString("000.0");

        var dataArray = Encoding.UTF8.GetBytes(_dataString);
        _steamManager.SendMessageToSocketServer(dataArray);
    }

    public void ReceiveNetworkedCharacterData(byte[] dataArray)
    {
        // Change a character position based on ID.
        _receivedSteamID = ulong.Parse(Encoding.UTF8.GetString(dataArray, 3, 17));
        
        _receivedPlayerPositionVector.x = float.Parse(Encoding.UTF8.GetString(dataArray, 20, 8));
        _receivedPlayerPositionVector.y = float.Parse(Encoding.UTF8.GetString(dataArray, 28, 8));
        _receivedPlayerPositionVector.z = float.Parse(Encoding.UTF8.GetString(dataArray, 36, 8));
        _receivedPlayerRotationVectorY = float.Parse(Encoding.UTF8.GetString(dataArray, 44, 5));

        var characterTransform = _clientDataManager.players[_receivedSteamID].character.transform;
        characterTransform.position = _receivedPlayerPositionVector;
        characterTransform.rotation = Quaternion.Euler(0f, _receivedPlayerRotationVectorY, 0f);
    }
}
