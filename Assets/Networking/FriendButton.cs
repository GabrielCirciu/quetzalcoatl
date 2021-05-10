using UnityEngine;
using Steamworks;

public class FriendButton : MonoBehaviour {
    public GameObject steamManagerObj;
    public SteamManager steamManager;
    public SteamId SteamID { get; set; }

    private void Awake() {
        steamManagerObj = GameObject.Find("SteamManager");
        steamManager = steamManagerObj.GetComponent<SteamManager>();
    }

    public void OnSelected() {
        steamManager.FriendSteamId = SteamID;
    }
}
