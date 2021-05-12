using System;
using UnityEngine;

public class DisconnectThing : MonoBehaviour
{
    public GameObject steamManagerObj;
    public SteamManager steamManager;

    private void Start()
    {
        steamManagerObj = GameObject.FindWithTag("SteamManager");
        steamManager = steamManagerObj.GetComponent<SteamManager>();
    }

    public void DisconnectButton()
    {
        steamManager.LeaveSteamSocketServer();
    }
}
