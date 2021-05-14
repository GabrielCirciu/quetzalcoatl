using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable RedundantOverriddenMember

// Socket Class for creating socket server, only for host utilization
public class SteamSocketManager : SocketManager
{
    private readonly byte[] _dataTypeCheck = new byte[1];
    private string _dataString;
    public override void OnConnecting(Connection connection, ConnectionInfo info) => base.OnConnecting(connection, info);

    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
		base.OnConnected(connection, info);
        Debug.Log($"SERVER: Client has connected.");
    }
	public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
		base.OnDisconnected(connection, info);
        Debug.Log($"SERVER: Client has disconnected");
    }
	public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        // Checks the first byte, if 33 ("!"), it has to save on server, otherwise just relay
        _dataTypeCheck[0] = System.Runtime.InteropServices.Marshal.ReadByte(data);
        if (_dataTypeCheck[0] != 33)
        {
            SteamManager.instance.RelaySocketMessageReceived(data, size, connection.Id);
        }
        else
        {
            SteamManager.instance.RelaySocketMessageReceived(data, size, connection.Id);
        }
        
        // Outputs extensive information about the data that it recieved, to Debug Log
        var dataBytes = new byte[size];
        System.Runtime.InteropServices.Marshal.Copy(data, dataBytes, 0, size);
        _dataString = System.Text.Encoding.UTF8.GetString(dataBytes);
        Debug.Log("SERVER: Data recieved " + $"( ConnectionID: {connection.Id}, SteamID: {identity.SteamId}, " +
                  $"Size: {size}, MessageNum: {messageNum}, RecvTime: {recvTime}, Channel: {channel}, " +
                  $"DataIntPtr: {data}, DataTypeCheck: {_dataTypeCheck[0]}, DataString: {_dataString}. Relaying...");
    }
}

// Connection Manager for enabling all player connections to socket server
public class SteamConnectionManager : ConnectionManager
{
    public override void OnConnecting(ConnectionInfo info) => base.OnConnecting(info);

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
        Debug.Log("CLIENT: Data recieved. Processing...");
        SteamManager.instance.ProcessMessageFromSocketServer(data, size);
    }
}