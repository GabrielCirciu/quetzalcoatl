using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterNetworkedStats : MonoBehaviour
{
    public static CharacterNetworkedStats instance;
    private SteamManager _steamManager;
    private ClientDataManager _clientDataManager;
    
    private void Awake() => instance = this;

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _clientDataManager = ClientDataManager.instance;
    }

    private void Update()
    {
        // Send data every few miliseconds
    }

    private void SendNetworkedCharacterData()
    {
        // #1 for position, need to send ID, position, rotation, velocity, animations, etc
    }

    public void ReceiveNetworkedCharacterData()
    {
        // Read someone's position, rotation, velocity, animations, all that
    }
}
