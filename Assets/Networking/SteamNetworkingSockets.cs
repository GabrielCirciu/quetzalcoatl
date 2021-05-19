using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable RedundantOverriddenMember

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager
{
    
    public override void OnConnecting(Connection connection, ConnectionInfo info) => base.OnConnecting(connection, info);

    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
		base.OnConnected(connection, info);
        Debug.Log($"SERVER: Client [ Connection ID: {connection.Id} ] has connected.\n");
    }
	public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
		base.OnDisconnected(connection, info);
        Debug.Log($"SERVER: Client [ Connection ID: {connection.Id} ] has disconnected\n");
        SteamManager.instance.RemoveFromPlayerDatabase(connection.Id);
    }
    public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        SteamManager.instance.RelaySocketMessageReceived(data, size, connection.Id);
    }
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager
{
    public override void OnConnecting(ConnectionInfo info) => base.OnConnecting(info);

    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        Debug.Log("CLIENT: Connected to server\n");
        SceneManager.LoadScene("CharacterAnimations");
    }
    public override void OnDisconnected(ConnectionInfo info)
    {
        base.OnDisconnected(info);
        Debug.Log("CLIENT: Disconnected from server\n");
        SceneManager.LoadScene("MenuScene");
    }
    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        SteamManager.instance.ProcessMessageFromSocketServer(data, size);
    }
}