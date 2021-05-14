using UnityEngine;

public class DisconnectThing : MonoBehaviour
{
    public void DisconnectButton()
    {
        SteamManager.instance.LeaveSteamSocketServer();
    }
}
