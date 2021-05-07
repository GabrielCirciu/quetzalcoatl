using Steamworks;
using Steamworks.Data;
using System;

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager {
    WorldManager worldManager;

    public override void OnConnecting(Connection connection, ConnectionInfo data) {
		base.OnConnecting(connection, data);
	}
    public override void OnConnected(Connection connection, ConnectionInfo data) {
		base.OnConnected(connection, data);
        // Send information to chat that you connected
        worldManager.StartMultiPlayerWorld();
	}
	public override void OnDisconnected(Connection connection, ConnectionInfo data) {
		base.OnDisconnected(connection, data);
        // worldManager.ReturnToMainMenu();
	}
	public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel) {
		SteamManager.Instance.RelaySocketMessageReceived(data, size, connection.Id);
	}
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager {
    WorldManager worldManager;
    
    public override void OnConnecting(ConnectionInfo info) {
        base.OnConnecting(info);
    }
    public override void OnConnected(ConnectionInfo info) {
        base.OnConnected(info);
        worldManager.StartMultiPlayerWorld();
    }
    public override void OnDisconnected(ConnectionInfo info) {
        base.OnDisconnected(info);
        worldManager.ReturnToMainMenu();
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) {
        SteamManager.Instance.ProcessMessageFromSocketServer(data, size);
    }
}