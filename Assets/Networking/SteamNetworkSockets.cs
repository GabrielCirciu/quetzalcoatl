using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager {

    public override void OnConnecting(Connection connection, ConnectionInfo info) {
		base.OnConnecting(connection, info);
        Debug.Log("SERVER: A client is CONNECTING");
	}
    public override void OnConnected(Connection connection, ConnectionInfo info) {
		base.OnConnected(connection, info);
        Debug.Log("SERVER: A client has CONNECTED");
	}
	public override void OnDisconnected(Connection connection, ConnectionInfo info) {
		base.OnDisconnected(connection, info);
        Debug.Log("SERVER: A client has DISCONNECTED");
	}
	public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel) {
		SteamManager.instance.RelaySocketMessageReceived(data, size, connection.Id);
        Debug.Log("SERVER: Data recieved. Relaying...");
	}
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager {
    private WorldManager _worldManager;
    
    public override void OnConnecting(ConnectionInfo info) {
        base.OnConnecting(info);
        Debug.Log($"CLIENT: Connecting");
    }
    public override void OnConnected(ConnectionInfo info) {
        base.OnConnected(info);
        Debug.Log("CLIENT: Connected");
        _worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        _worldManager.JoinMultiPlayerWorld();
    }
    public override void OnDisconnected(ConnectionInfo info) {
        base.OnDisconnected(info);
        Debug.Log("CLIENT: Disconnected");
        _worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        _worldManager.ReturnToMainMenu();
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) {
        SteamManager.instance.ProcessMessageFromSocketServer(data, size);
        Debug.Log("CLIENT: Data recieved. Processing...");
    }
}