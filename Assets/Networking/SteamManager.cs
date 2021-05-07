using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamManager : MonoBehaviour {
    public static SteamManager Instance;
    private static uint gameAppId = 480;
    private bool appHasQuit = false;
    private bool firstInstance = false;

    public SteamId PlayerSteamId { get; set; }
    public SteamId FriendSteamId { get; set; }

    public SteamSocketManager steamSocketManager = null;
    public SteamConnectionManager steamConnectionManager = null;
    public bool activeSteamSocketServer = false;
    public bool activeSteamSocketConnection = false;

    DataManager dataManager;
    
    void Awake(){
        if ( Instance == null ){
            firstInstance = true;
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            try{ 
                SteamClient.Init(gameAppId, true);
                if ( !SteamClient.IsValid ) throw new Exception();
                PlayerSteamId = SteamClient.SteamId;
                SteamNetworkingUtils.InitRelayNetworkAccess();
            }
            catch ( Exception e ){ Debug.LogError($"Steam not initialized. Exception: {e}"); }
        }
        else if ( Instance != this ) { Destroy(gameObject); }
    }

    void Start() { }

    void Update() {
        SteamClient.RunCallbacks();
        try {
            if ( activeSteamSocketServer ) { steamSocketManager.Receive(); }
            if ( activeSteamSocketConnection ) { steamConnectionManager.Receive(); }
        }
        catch { Debug.LogError("Error recieving data on socket/connection"); }
    }

    public void ActivateDataManager() {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void CreateSteamSocketServer() {
        steamSocketManager = SteamNetworkingSockets.CreateRelaySocket<SteamSocketManager>(0);
        steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(PlayerSteamId);
        activeSteamSocketServer = true;
        activeSteamSocketConnection = true;
        if (steamConnectionManager != null) {
            Debug.Log("Server created and joined");
        }
    }

    public void JoinSteamSocketServer() {
        steamConnectionManager = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(FriendSteamId, 0);
        activeSteamSocketServer = false;
        activeSteamSocketConnection = true;
        if (steamConnectionManager != null) Debug.Log("Server joined");
    }

    public void LeaveSteamSocketServer() {
        activeSteamSocketServer = false;
        activeSteamSocketConnection = false;
        try {
            steamConnectionManager.Close();
            steamSocketManager.Close();
        }
        catch { Debug.Log("Error closing socket server / connection manager"); }
    }

    public void RelaySocketMessageReceived(IntPtr message, int size, uint connectionSendingMessageId) {
        try { // Loop to only send messages to socket server members who are not the one that sent the message
            for (int i = 0; i < steamSocketManager.Connected.Count; i++) {
                if (steamSocketManager.Connected[i].Id != connectionSendingMessageId) {
                    Result success = steamSocketManager.Connected[i].SendMessage(message, size);
                    if (success != Result.OK) { Result retry = steamSocketManager.Connected[i].SendMessage(message, size); }
                    else { Debug.Log("Socket Message relayed from Server"); }
                }
            }
        }
        catch { Debug.Log("Unable to relay socket server message"); }
    }

    public bool SendMessageToSocketServer(byte[] messageToSend) {
        try { // Convert string/byte[] message into IntPtr data type for efficient message send / garbage management
            int sizeOfMessage = messageToSend.Length;
            IntPtr intPtrMessage = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeOfMessage);
            System.Runtime.InteropServices.Marshal.Copy(messageToSend, 0, intPtrMessage, sizeOfMessage);
            Result success = steamConnectionManager.Connection.SendMessage(intPtrMessage, sizeOfMessage, SendType.Reliable);
            if (success == Result.OK) {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage); // Free up memory at pointer
                return true;
            }
            else { // RETRY
                Result retry = steamConnectionManager.Connection.SendMessage(intPtrMessage, sizeOfMessage, SendType.Reliable);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage); // Free up memory at pointer
                if (retry == Result.OK) { return true; }
                return false;
            }
        }
        catch (Exception e) {
            Debug.Log($"Unable to send message to socket server. Exception: {e}");
            return false;
        }
    }

    public void ProcessMessageFromSocketServer(IntPtr messageIntPtr, int dataBlockSize) {
        try {
            byte[] dataArray = new byte[dataBlockSize];
            System.Runtime.InteropServices.Marshal.Copy(messageIntPtr, dataArray, 0, dataBlockSize);
            //dataManager.ProcessRecievedData(dataArray);
        }
        catch { Debug.Log("Unable to process message from socket server"); }
    }

    void OnDisable(){ if (firstInstance) gameCleanup(); }

    void OnDestroy(){ if (firstInstance) gameCleanup(); }

    void OnApplicationQuit(){ if (firstInstance) gameCleanup(); }

    void gameCleanup(){
        if (!appHasQuit) {
            appHasQuit = true;
            SteamClient.Shutdown();
        }
    }
}