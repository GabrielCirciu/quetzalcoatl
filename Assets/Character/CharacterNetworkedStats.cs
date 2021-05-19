using System;
using System.Text;
using Steamworks;
using UnityEngine;

public class CharacterNetworkedStats : MonoBehaviour
{
    // ASCII: N - No save, P - Player, P - Position (send ID, position, rotation, etc.)
    private const string DataIdentifier = "NPP";
    
    public static CharacterNetworkedStats instance;
    private SteamManager _steamManager;
    private ClientDataManager _clientDataManager;
    private ulong _receivedSteamId;
    private Transform _localPlayerTransform, _receivedPlayerTransform;
    private Vector3 _localPosV3, _receivedPosV3;
    private float _localRotEulerY, _receivedRotEulerY;
    private byte[] _dataIdArray, _localSteamIdArray, _receivedSteamIdArray, _finalDataArray;
    private float[] _localPlayerFloatArray, _receivedPlayerFloatArray;
    
    /* --- FOR TESTING PURPOSES ---
    public GameObject cubey;
    private Transform _cubeyTransform;
    private Vector3 _cubeyPosV3;
    */

    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _clientDataManager = ClientDataManager.instance;
        _localPlayerTransform = GetComponent<Transform>();
        
        _dataIdArray = Encoding.UTF8.GetBytes(DataIdentifier);
        _localSteamIdArray = BitConverter.GetBytes(SteamClient.SteamId.Value);

        /* --- FOR TESTING PURPOSES ---
        _cubeyTransform = cubey.GetComponent<Transform>();
        */

        _receivedPlayerFloatArray = new float[4];
        InvokeRepeating(nameof(SendNetworkedCharacterData), 3.0f, 0.1f);
    }

    private void SendNetworkedCharacterData()
    {
        _localPosV3 = _localPlayerTransform.position;
        _localRotEulerY = _localPlayerTransform.rotation.eulerAngles.y;
        _localPlayerFloatArray = new[] {_localPosV3.x, _localPosV3.y, _localPosV3.z, _localRotEulerY};
        
        _finalDataArray = new byte[3 + 8 + 16];
        Buffer.BlockCopy(_dataIdArray, 0, _finalDataArray, 0, 3);
        Buffer.BlockCopy(_localSteamIdArray, 0, _finalDataArray, 3, 8);
        Buffer.BlockCopy(_localPlayerFloatArray, 0, _finalDataArray, 11, 16);
        
        /* --- FOR TESTING PURPOSES ---
        // Run backwards call as if we were the receiver
        _receivedSteamId = BitConverter.ToUInt64(_finalDataArray, 3);
        Buffer.BlockCopy(_finalDataArray, 11, _receivedPlayerFloatArray, 0, 16);
        
        // Move Cubey
        _cubeyPosV3 = new Vector3(_receivedPlayerFloatArray[0], 3, _receivedPlayerFloatArray[2]);
        _cubeyTransform.position = _cubeyPosV3;
        _cubeyTransform.rotation = Quaternion.Euler(0f, _receivedPlayerFloatArray[3], 0f);
        */

        _steamManager.SendMessageToSocketServer(_finalDataArray);
    }

    public void ReceiveNetworkedCharacterData(byte[] dataArray)
    {
        _receivedSteamId = BitConverter.ToUInt64(dataArray, 3);
        Buffer.BlockCopy(dataArray, 11, _receivedPlayerFloatArray, 0, 16);
        _receivedPlayerTransform = _clientDataManager.players[_receivedSteamId].character.transform;
        _receivedPosV3 = new Vector3(_receivedPlayerFloatArray[0], _receivedPlayerFloatArray[1], _receivedPlayerFloatArray[2]);
        _receivedPlayerTransform.position = _receivedPosV3;
        _receivedPlayerTransform.rotation = Quaternion.Euler(0f, _receivedPlayerFloatArray[3], 0f);
    }
}
