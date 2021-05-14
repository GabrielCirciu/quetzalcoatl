using UnityEngine;
using Steamworks;

public class FriendButton : MonoBehaviour {
    public SteamId SteamID { get; set; }
    
    public void OnSelected() {
        SteamManager.instance.FriendSteamId = SteamID;
    }
}
