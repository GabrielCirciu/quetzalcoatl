using Steamworks;
using System;
using UnityEngine;

public class SteamManager : MonoBehaviour {
    public static SteamManager instance;
    private ServerDataManager _serverDataManager;
    private ClientDataManager _clientDataManager;
    private const uint GameAppId = 480;
    private bool _appHasQuit;
    private bool _firstInstance;    

    private SteamId PlayerSteamId { get; set; }
    public SteamId FriendSteamId { get; set; }

    private SteamSocketManager _steamSocketManager;
    private SteamConnectionManager _steamConnectionManager;
    public bool activeSteamSocketServer;
    public bool activeSteamSocketConnection;
    
    private readonly byte[] _dataTypeCheck = new byte[1];

    private void Awake()
    {
        if ( instance == null )
        {
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
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        _serverDataManager = ServerDataManager.instance;
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
        try
        {
            if (activeSteamSocketServer) _steamSocketManager.Receive();
            if (activeSteamSocketConnection) _steamConnectionManager.Receive();
        }
        catch (Exception e) { Debug.LogError($"ERROR: Error receiving data! Exception: {e}"); }
    }

    public void CreateSteamSocketServer()
    {
        Debug.Log("SERVER: Creating Steam Socket Server");
        
        _steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>();
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        
        if (_steamSocketManager != null) activeSteamSocketServer = true;
            else Debug.LogError("SERVER: Socket Manager = null");
        if (_steamConnectionManager != null) activeSteamSocketConnection = true;
            else Debug.LogError("SERVER: Connection Manager = null");
        
        _serverDataManager.ClearPlayerDatabase();
    }

    public void JoinSteamSocketServer()
    {
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId);
        if (_steamConnectionManager != null) activeSteamSocketConnection = true;
            else Debug.LogError("CLIENT: Connection Manager = null");
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

    public void RelaySocketMessageReceived(IntPtr dataPtr, int size, uint connectionID)
    {
        // Checks the first byte, if 33 ("!"), it has to save on server, otherwise just relay
        _dataTypeCheck[0] = System.Runtime.InteropServices.Marshal.ReadByte(dataPtr);
        if (_dataTypeCheck[0] == 33) _serverDataManager.ProcessRecievedData(dataPtr, size, connectionID);
        
        try 
        {
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++)
            {
                if (_steamSocketManager.Connected[i].Id == connectionID) continue;
                var success = _steamSocketManager.Connected[i].SendMessage(dataPtr, size);
                if (success != Result.OK) Debug.LogError("SERVER: Socket Message sending result not OK", this);
            }
        }
        catch (Exception e) { Debug.LogError($"SERVER: Error relaying data! Exception: {e}", this); }
    }

    public bool SendMessageToSocketServer(byte[] messageToSend)
    {
        try
        {
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
    
    public void ProcessMessageFromSocketServer(IntPtr messageIntPtr, int dataBlockSize)
    {
        try
        {
            var dataArray = new byte[dataBlockSize];
            System.Runtime.InteropServices.Marshal.Copy(messageIntPtr, dataArray, 0, dataBlockSize);
            _clientDataManager.ProcessRecievedData(dataArray);
        }
        catch (Exception e) { Debug.LogError($"CLIENT: Error processing data! Exception: {e}"); }
    }

    public void EnableClientDataManager()
    {
        _clientDataManager = ClientDataManager.instance;
    }
    
    private void OnDisable()
    {
        if (_firstInstance) GameCleanup();
    }

    private void OnDestroy()
    {
        if (_firstInstance) GameCleanup();
    }

    private void OnApplicationQuit()
    {
        if (_firstInstance) GameCleanup();
    }

    private void GameCleanup()
    {
        if (!_appHasQuit)
        {
            _appHasQuit = true;
            SteamClient.Shutdown();
        }
    }
}