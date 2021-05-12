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
        Debug.Log($"SERVER: Client is connecting.");
	}
    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
		base.OnConnected(connection, info);
        Debug.Log($"SERVER: Client has connected.");
        GameObject.Find("SteamManager").GetComponent<SteamManager>().UpdatePlayerList();
	}
	public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
		base.OnDisconnected(connection, info);
        Debug.Log($"SERVER: Client has disconnected");
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
        Debug.Log("CLIENT: Connecting to server");
    }
    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        Debug.Log("CLIENT: Connected to server");
        SceneManager.LoadScene("CharacterAnimations");
    }
    public override void OnDisconnected(ConnectionInfo info)
    {
        base.OnDisconnected(info);
        Debug.Log("CLIENT: Disconnected from server");
        SceneManager.LoadScene("MenuScene");
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        SteamManager.instance.ProcessMessageFromSocketServer(data, size);
        Debug.Log("CLIENT: Data recieved. Processing...");
    }
}