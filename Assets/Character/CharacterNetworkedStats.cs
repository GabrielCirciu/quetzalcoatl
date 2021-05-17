using System.Timers;
using Steamworks;
using UnityEngine;

public class CharacterNetworkedStats : MonoBehaviour
{
    public static CharacterNetworkedStats instance;
    private SteamManager _steamManager;
    private ClientDataManager _clientDataManager;
    private ulong _steamID;
    private string _dataString;
    private Transform _positionTransform;
    private Vector3 _positionVector;

    private Timer _timer;
    
    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _clientDataManager = ClientDataManager.instance;
        _steamID = SteamClient.SteamId.Value;
        
        _positionTransform = GetComponent<Transform>();
        _positionVector = _positionTransform.position;
        Debug.Log($"Starting position: {_positionVector}");

        _timer = new Timer(1000);
        _timer.Elapsed += TimeElapsed;
        _timer.Enabled = true;
    }

    private void TimeElapsed(object sender, ElapsedEventArgs e)
    {
        SendNetworkedCharacterData();
    }

    private void SendNetworkedCharacterData()
    {
        // TODO
        // #1 for position, need to send ID, position, rotation, velocity, animations, etc
        //_dataString = "#1"+_steamID+" --- "+_positionVector.x+";"+_positionVector.y+";"+_positionVector.z;
        Debug.Log($"CLIENT: Sending character data...\nString: ()\n");
    }

    public void ReceiveNetworkedCharacterData()
    {
        // Read someone's position, rotation, velocity, animations, all that
    }
}
