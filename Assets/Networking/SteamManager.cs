using Steamworks;
using System;
using UnityEngine;
using System.Text;
using static System.Runtime.InteropServices.Marshal;

public class SteamManager : MonoBehaviour
{
    // ASCII: S - Save, P - Player, L - Left server
    private const string RemovePlayerDataIdentifier = "SNP";
    
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
    private byte[] _newClientDataIdArray;

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
                Debug.LogError($"Steam not initialized. Exception: {e}", this);
                Application.Quit();
            }
        }
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        _serverDataManager = ServerDataManager.instance;
        _newClientDataIdArray = Encoding.UTF8.GetBytes(RemovePlayerDataIdentifier);
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
        try
        {
            if (activeSteamSocketServer) _steamSocketManager.Receive();
            if (activeSteamSocketConnection) _steamConnectionManager.Receive();
        }
        catch (Exception e) { Debug.LogError($"ERROR: Error receiving data! Exception: {e}", this); }
    }

    public void CreateSteamSocketServer()
    {
        Debug.Log("SERVER: Creating Steam Socket Server\n");
        
        _steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>();
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        
        if (_steamSocketManager != null) activeSteamSocketServer = true;
            else Debug.LogError("SERVER: Socket Manager = null", this);
        if (_steamConnectionManager != null) activeSteamSocketConnection = true;
            else Debug.LogError("SERVER: Connection Manager = null", this);
        
        _serverDataManager.ClearPlayerDatabase();
    }

    public void JoinSteamSocketServer()
    {
        _steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId);
        if (_steamConnectionManager != null) activeSteamSocketConnection = true;
            else Debug.LogError("CLIENT: Connection Manager = null", this);
    }

    public void LeaveSteamSocketServer()
    {
        if (activeSteamSocketConnection)
        {
            activeSteamSocketConnection = false;
            _steamConnectionManager.Close();
        }

        if (!activeSteamSocketServer) return;
        activeSteamSocketServer = false;
        _steamSocketManager.Close();
    }

    public void RelaySocketMessageReceived(IntPtr dataIntPtr, int dataSize, uint connectionID)
    {
        // Check first byte, if ASCII: S - Save data
        _dataTypeCheck[0] = ReadByte(dataIntPtr);
        if (_dataTypeCheck[0] == 83)
            _serverDataManager.ProcessReceivedSaveData(dataIntPtr, dataSize, connectionID);
        
        try 
        {
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++)
            {
                if (_steamSocketManager.Connected[i].Id == connectionID) continue;
                var success = _steamSocketManager.Connected[i].SendMessage(dataIntPtr, dataSize);
                if (success != Result.OK) Debug.LogError("SERVER: Socket Message sending result not OK", this);
            }
        }
        catch (Exception e) { Debug.LogError($"SERVER: Error relaying data! Exception: {e}", this); }
    }

    public bool SendMessageToSocketServer(byte[] dataArray)
    {
        try
        {
            var dataSize = dataArray.Length;
            var dataIntPtr = AllocHGlobal(dataSize);
            Copy(dataArray, 0, dataIntPtr, dataSize);
            var success = _steamConnectionManager.Connection.SendMessage(dataIntPtr, dataSize);
            if (success == Result.OK)
            {
                FreeHGlobal(dataIntPtr);
                return true;
            }
            Debug.LogError("CLIENT: Sending result returned not OK", this);
            return false;
        }
        catch (Exception e) {
            Debug.LogError($"CLIENT: Error sending data! Exception: {e}", this);
            return false;
        }
    }
    
    public void ProcessMessageFromSocketServer(IntPtr dataIntPtr, int dataSize)
    {
        try
        {
            var dataArray = new byte[dataSize];
            Copy(dataIntPtr, dataArray, 0, dataSize);
            _clientDataManager.ProcessRecievedData(dataArray);
        }
        catch (Exception e) { Debug.LogError($"CLIENT: Error processing data! Exception: {e}", this); }
    }
    
    public void RemoveFromPlayerDatabase(uint connectionID)
    {
        Debug.Log($"SERVER: Removing player [ ID: {_serverDataManager.players[connectionID].id}, Name: {_serverDataManager.players[connectionID].name} ] from database...");
        try
        {
            var finalDataArray = new byte[11];
            Buffer.BlockCopy(_newClientDataIdArray, 0, finalDataArray, 0, 3);
            var playerIdArray = BitConverter.GetBytes(_serverDataManager.players[connectionID].id);
            Buffer.BlockCopy(playerIdArray, 0, finalDataArray, 3, 8);
            const int finalDataSize = 11;
            var dataIntPtr = AllocHGlobal(finalDataSize);
            Copy(finalDataArray, 0, dataIntPtr, finalDataSize);
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++)
            {
                var success = _steamSocketManager.Connected[i].SendMessage(dataIntPtr, finalDataSize);
                if (success != Result.OK) Debug.LogError("SERVER: Socket Message sending result not OK", this);
            }
            FreeHGlobal(dataIntPtr);
        }
        catch (Exception e) { Debug.LogError($"SERVER: Error sending data! Exception: {e}", this); }

        _serverDataManager.players.Remove(connectionID);
        _serverDataManager.ShowNewPlayerList();
    }

    public void SendDataToNewPlayer(uint connectionId, byte[] dataArray)
    {
        try
        {
            var dataSize = dataArray.Length;
            var dataIntPtr = AllocHGlobal(dataSize);
            Copy(dataArray, 0, dataIntPtr, dataSize);
            for (var i = 0; i < _steamSocketManager.Connected.Count; i++)
            {
                if (_steamSocketManager.Connected[i].Id != connectionId) continue;
                var success = _steamSocketManager.Connected[i].SendMessage(dataIntPtr, dataSize);
                if (success != Result.OK) Debug.LogError("SERVER: Socket Message sending result not OK", this);
            }
            FreeHGlobal(dataIntPtr);
        }
        catch (Exception e) { Debug.LogError($"SERVER: Error sending data! Exception: {e}", this); }
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
        if (_appHasQuit) return;
        _appHasQuit = true;
        SteamClient.Shutdown();
    }
}