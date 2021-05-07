using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager {

    public override void OnConnecting(Connection connection, ConnectionInfo data) {
		base.OnConnecting(connection, data);
        Debug.Log("Connecting to server as host");
	}
    public override void OnConnected(Connection connection, ConnectionInfo data) {
		base.OnConnected(connection, data);
        Debug.Log("Connected to server as host");
	}
	public override void OnDisconnected(Connection connection, ConnectionInfo data) {
		base.OnDisconnected(connection, data);
        Debug.Log("Server has been shut down");
	}
	public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel) {
		SteamManager.Instance.RelaySocketMessageReceived(data, size, connection.Id);
	}
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager {
    public WorldManager worldManager;
    
    public override void OnConnecting(ConnectionInfo info) {
        base.OnConnecting(info);
        Debug.Log("Connecting to server as client");
    }
    public override void OnConnected(ConnectionInfo info) {
        base.OnConnected(info);
        Debug.Log("Connected to server as client");
        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        worldManager.JoinMultiPlayerWorld();
    }
    public override void OnDisconnected(ConnectionInfo info) {
        base.OnDisconnected(info);
        Debug.Log("Disconnected from server");
        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        worldManager.ReturnToMainMenu();
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) {
        SteamManager.Instance.ProcessMessageFromSocketServer(data, size);
    }
}