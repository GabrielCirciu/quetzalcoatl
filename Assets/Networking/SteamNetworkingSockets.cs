using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager
{
    public override void OnConnecting(Connection connection, ConnectionInfo info)
    {
		base.OnConnecting(connection, info);
        Debug.Log($"SERVER: {info.Identity.SteamId.AccountId} is connecting.");
	}
    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
		base.OnConnected(connection, info);
        Debug.Log($"SERVER: {info.Identity.SteamId.AccountId} has connected.");
	}
	public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
		base.OnDisconnected(connection, info);
        Debug.Log($"SERVER: {info.Identity.SteamId.AccountId} has disconnected");
	}
	public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
		SteamManager.instance.RelaySocketMessageReceived(data, size, connection.Id);
        Debug.Log("SERVER: Data recieved. Relaying...");
	}
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager
{
    public override void OnConnecting(ConnectionInfo info)
    {
        base.OnConnecting(info);
        Debug.Log($"CLIENT: Connecting to {info.Identity.SteamId.AccountId}");
    }
    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        Debug.Log($"CLIENT: Connected to {info.Identity.SteamId.AccountId}");
        SceneManager.LoadScene("TestingScene2");
    }
    public override void OnDisconnected(ConnectionInfo info)
    {
        SceneManager.LoadScene("MenuScene");
        base.OnDisconnected(info);
        Debug.Log($"CLIENT: Disconnected from {info.Address}");
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        SteamManager.instance.ProcessMessageFromSocketServer(data, size);
        Debug.Log("CLIENT: Data recieved. Processing...");
    }
}