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
    
    private void Awake()
    {
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

    private void Update()
    {
        SteamClient.RunCallbacks();
        try {
            if ( activeSteamSocketServer ) { _steamSocketManager.Receive(); }
            if ( activeSteamSocketConnection ) { _steamConnectionManager.Receive(); }
        }
        catch { Debug.LogError("SERVER/CLIENT: Error receiving data"); }
    }

    public void CreateSteamSocketServer()
    {
        Debug.Log("SERVER: Attempting to create server");
        _steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>();
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        if (_steamSocketManager != null)
        {
            activeSteamSocketServer = true;
        }
        else
        {
            Debug.LogError("SERVER: Socket Manager = null");
        }

        if (_steamConnectionManager != null)
        {
            activeSteamSocketConnection = true;
        }
        else
        {
            Debug.LogError("SERVER: Connection Manager = null");
        }
    }

    public void JoinSteamSocketServer()
    {
        Debug.Log("CLIENT: Attempting to join server");
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId);
        if (_steamConnectionManager != null)
        {
            activeSteamSocketConnection = true;
        }
        else
        {
            Debug.LogError("CLIENT: Connection Manager = null");
        }
    }

    public void LeaveSteamSocketServer()
    {
        if (activeSteamSocketConnection)
        {
            activeSteamSocketConnection = false;
            _steamConnectionManager.Close();
        }
        if (activeSteamSocketServer)
        {
            activeSteamSocketServer = false;
            _steamSocketManager.Close();
        }
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
            Debug.LogError($"CLIENT: Result returned unsuccessful");
            return false;
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