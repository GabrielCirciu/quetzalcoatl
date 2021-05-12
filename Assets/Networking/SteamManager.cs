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
            try
            {
                SteamClient.Init(GameAppId);
                if (!SteamClient.IsValid) throw new Exception();
                PlayerSteamId = SteamClient.SteamId;
                SteamNetworkingUtils.InitRelayNetworkAccess();
            }
            catch (Exception e)
            {
                Debug.LogError($"Steam not initialized. Exception: {e}");
                Application.Quit();
            }
        }
        else if ( instance != this ) { Destroy(gameObject); }
    }

    private void Update() {
        SteamClient.RunCallbacks();
        try {
            if ( activeSteamSocketServer ) { _steamSocketManager.Receive(); }
            if ( activeSteamSocketConnection ) { _steamConnectionManager.Receive(); }
        }
        catch { Debug.LogError("SERVER/CLIENT: Error receiving data"); }
    }

    public void ActivateDataManager() {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void CreateSteamSocketServer() {
        Debug.Log("SERVER: Attempting to create socket");
        _steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>();
        if (_steamSocketManager == null) Debug.LogError("SERVER: Socket Manager = null");
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        if (_steamConnectionManager == null) Debug.LogError("SERVER: Connection Manager = null");
        activeSteamSocketServer = true;
        activeSteamSocketConnection = true;
    }

    public void JoinSteamSocketServer() {
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId);
        if (_steamConnectionManager == null) Debug.LogError("CLIENT: Connection Manager = null");
        activeSteamSocketServer = false;
        activeSteamSocketConnection = true;
    }

    public void LeaveSteamSocketServer()
    {
        try
        {
            if (activeSteamSocketConnection)
            {
                Debug.Log("CLIENT: Attempting to leave server");
                _steamConnectionManager.Close();
                _steamConnectionManager = null;
                activeSteamSocketConnection = false;
            }
            if (activeSteamSocketServer)
            {
                Debug.Log("SERVER: Attempting to close Socket");
                _steamSocketManager.Close();
                _steamSocketManager = null;
                activeSteamSocketServer = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SERVER/CLIENT: Error closing connection! Exception {e}");
        }
        //if (_steamSocketManager != null) Debug.LogError("SERVER: Socket Manager still active");
        //if (_steamConnectionManager != null) Debug.LogError("SERVER: Connection Manager still active");
    }

    public void RelaySocketMessageReceived(IntPtr message, int size, uint connectionSendingMessageId) {
        try {
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++) {
                if (_steamSocketManager.Connected[i].Id != connectionSendingMessageId)
                {
                    var success = _steamSocketManager.Connected[i].SendMessage(message, size);
                    if (success != Result.OK) Debug.LogError("SERVER: Socket Message couldn't be relayed");
                }
            }
        }
        catch { Debug.LogError("SERVER: Socket Message couldn't be relayed"); }
    }

    public bool SendMessageToSocketServer(byte[] messageToSend) {
        try {
            var sizeOfMessage = messageToSend.Length;
            var intPtrMessage = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeOfMessage);
            System.Runtime.InteropServices.Marshal.Copy(messageToSend, 0, intPtrMessage, sizeOfMessage);
            var success = _steamConnectionManager.Connection.SendMessage(intPtrMessage, sizeOfMessage);
            if (success == Result.OK)
            {
                // Free up memory at pointer
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage);
                return true;
            }
            else
            {
                Debug.LogError($"CLIENT: Result returned unsuccessful");
                return false;
            }
        }
        catch (Exception e) {
            Debug.LogError($"CLIENT: Error sending data! Exception: {e}");
            return false;
        }
    }

    public void ProcessMessageFromSocketServer(IntPtr messageIntPtr, int dataBlockSize) {
        try
        {
            var dataArray = new byte[dataBlockSize];
            System.Runtime.InteropServices.Marshal.Copy(messageIntPtr, dataArray, 0, dataBlockSize);
            dataManager.ProcessRecievedData(dataArray);
        }
        catch (Exception e)
        {
            Debug.LogError($"CLIENT: Error processing data! Exception: {e}");
        }
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