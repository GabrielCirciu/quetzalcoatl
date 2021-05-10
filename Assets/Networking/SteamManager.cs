using Steamworks;
using System;
using UnityEngine;

public class SteamManager : MonoBehaviour {
    public static SteamManager instance;
    private const uint GameAppId = 480;
    private bool _appHasQuit;
    private bool _firstInstance;

    private SteamId PlayerSteamId { get; set; }
    public SteamId FriendSteamId { get; set; }

    private SteamSocketManager _steamSocketManager;
    private SteamConnectionManager _steamConnectionManager;
    public bool activeSteamSocketServer;
    public bool activeSteamSocketConnection;

    public DataManager dataManager;

    private void Awake(){
        if ( instance == null ){
            _firstInstance = true;
            DontDestroyOnLoad(gameObject);
            instance = this;
            try{ 
                SteamClient.Init(GameAppId);
                if ( !SteamClient.IsValid ) throw new Exception();
                PlayerSteamId = SteamClient.SteamId;
                SteamNetworkingUtils.InitRelayNetworkAccess();
            }
            catch ( Exception e ){ Debug.LogError($"Steam not initialized. Exception: {e}"); }
        }
        else if ( instance != this ) { Destroy(gameObject); }
    }

    private void Update() {
        SteamClient.RunCallbacks();
        try {
            if ( activeSteamSocketServer ) { _steamSocketManager.Receive(); }
            if ( activeSteamSocketConnection ) { _steamConnectionManager.Receive(); }
        }
        catch { Debug.LogError("Error recieving data on socket/connection"); }
    }

    public void ActivateDataManager() {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void CreateSteamSocketServer() {
        _steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>();
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        activeSteamSocketServer = true;
        activeSteamSocketConnection = true;
        if (_steamConnectionManager != null) Debug.Log("Attempting to create a server");
    }

    public void JoinSteamSocketServer() {
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId);
        activeSteamSocketServer = false;
        activeSteamSocketConnection = true;
        if (_steamConnectionManager != null) Debug.Log("Attempting to join a server");
    }

    public void LeaveSteamSocketServer() {
        try {
            _steamSocketManager.Close();
            _steamConnectionManager.Close();
            Debug.Log("Left the server");
        }
        catch { Debug.Log("Error closing socket server / connection manager"); }
        activeSteamSocketServer = false;
        activeSteamSocketConnection = false;
    }

    public void RelaySocketMessageReceived(IntPtr message, int size, uint connectionSendingMessageId) {
        try {
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++) {
                if (_steamSocketManager.Connected[i].Id != connectionSendingMessageId) {
                    var success = _steamSocketManager.Connected[i].SendMessage(message, size);
                    if (success != Result.OK) Debug.Log("Socket Message could not be relayed");
                    else Debug.Log("Socket Message relayed from Server");
                }
            }
        }
        catch { Debug.Log("Unable to relay socket server message"); }
    }

    public bool SendMessageToSocketServer(byte[] messageToSend) {
        Debug.Log("Data sent to server");
        try {
            var sizeOfMessage = messageToSend.Length;
            var intPtrMessage = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeOfMessage);
            System.Runtime.InteropServices.Marshal.Copy(messageToSend, 0, intPtrMessage, sizeOfMessage);
            var success = _steamConnectionManager.Connection.SendMessage(intPtrMessage, sizeOfMessage);
            if (success == Result.OK) {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage); // Free up memory at pointer
                return true;
            }
            else return false;
        }
        catch (Exception e) {
            Debug.Log($"Unable to send message to socket server. Exception: {e}");
            return false;
        }
    }

    public void ProcessMessageFromSocketServer(IntPtr messageIntPtr, int dataBlockSize) {
        try {
            var dataArray = new byte[dataBlockSize];
            System.Runtime.InteropServices.Marshal.Copy(messageIntPtr, dataArray, 0, dataBlockSize);
            dataManager.ProcessRecievedData(dataArray);
        }
        catch { Debug.Log("Unable to process message from socket server"); }
    }

    private void OnDisable(){ if (_firstInstance) GameCleanup(); }

    private void OnDestroy(){ if (_firstInstance) GameCleanup(); }

    private void OnApplicationQuit(){ if (_firstInstance) GameCleanup(); }

    private void GameCleanup(){
        if (!_appHasQuit) {
            _appHasQuit = true;
            SteamClient.Shutdown();
        }
    }
}