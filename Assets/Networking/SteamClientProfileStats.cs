using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SteamClientProfileStats : MonoBehaviour {
    TMP_Text displayName;
    public GameObject displayNameObj;
    public RawImage displaySteamImage;

    void Start() {
        if (SteamClient.IsValid) {
            displayName = displayNameObj.GetComponent<TMP_Text>();
            GetSteamImage();
        }
        else Application.Quit();
    }

    async void GetSteamImage() {
        displayName.text = SteamClient.Name;
        var spImageData = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        Texture2D spImage = new Texture2D((int)spImageData.Value.Width, (int)spImageData.Value.Height, TextureFormat.RGBA32, false, false);
        spImage.LoadRawTextureData(spImageData.Value.Data);
        spImage.Apply();
        displaySteamImage.texture = spImage;
    }
}
