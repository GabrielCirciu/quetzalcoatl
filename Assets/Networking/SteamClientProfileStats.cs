using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SteamClientProfileStats : MonoBehaviour {
    TMP_Text _displayName;
    public GameObject displayNameObj;
    public RawImage displaySteamImage;

    void Start() {
        if (SteamClient.IsValid) {
            _displayName = displayNameObj.GetComponent<TMP_Text>();
            GetSteamImage();
        }
        else Application.Quit();
    }

    async void GetSteamImage() {
        _displayName.text = SteamClient.Name;
        var spImageData = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        var spImage = new Texture2D((int)spImageData.Value.Width, (int)spImageData.Value.Height, TextureFormat.RGBA32, false, false);
        spImage.LoadRawTextureData(spImageData.Value.Data);
        spImage.Apply();
        displaySteamImage.texture = spImage;
    }
}
