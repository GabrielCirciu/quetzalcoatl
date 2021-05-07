using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class FriendButton : MonoBehaviour {
    public GameObject steamManagerObj;
    public SteamManager steamManager;
    public SteamId steamID { get; set; }

    void Awake() {
        steamManagerObj = GameObject.Find("SteamManager");
        steamManager = steamManagerObj.GetComponent<SteamManager>();
    }

    public void OnSelected() {
        steamManager.FriendSteamId = steamID;
    }
}
