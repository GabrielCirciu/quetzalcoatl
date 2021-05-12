using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendListManager : MonoBehaviour
{
    private int _friendListLength;
    public GameObject friendListContentPanel, friendObj;
    private readonly List<SteamFriendInfo> _friendList = new List<SteamFriendInfo>();
    private class SteamFriendInfo
    {
        public TMP_Text nameText;
        public RawImage avatar;
    }
    public FriendButton friendButton;

    public void GetSteamFriends()
    {
        if ( _friendListLength != 0 )
        {
            for ( var i = 0; i < _friendListLength; i++ )
            {
                Destroy(_friendList[0].nameText.transform.parent.gameObject);
                _friendList.Remove(_friendList[0]);
            }
            _friendListLength = 0;
        }
        GetServerList();
        //GetPlayingFriends();
        //GetOnlineFriends();
        //GetOfflineFriends();
    }

    private static void GetServerList()
    {
        Debug.Log("Getting server list");
        var request = new Steamworks.ServerList.Internet();
        Debug.Log("Getting server list");
        request.RunQueryAsync(30);
        Debug.Log("Running Async Query");
        foreach (var serverInfo in request.Responsive)
        {
            Debug.Log(serverInfo.Name);
        }
        Debug.Log("Finished displaying servers");
    }

    private void GetPlayingFriends()
    {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if (friend.IsPlayingThisGame)
            {
                PopulateFriendList(friend, 1);
            }
        }
    }

    private void GetOnlineFriends() {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if (friend.IsOnline && !friend.IsPlayingThisGame)
            {
                PopulateFriendList(friend, 2);
            }
        }
    }

    private void GetOfflineFriends() {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if ( !friend.IsOnline ) PopulateFriendList(friend, 3);
        }
    }

    private async void PopulateFriendList(Friend friend, int status) {
        _friendListLength++;
        await friend.RequestInfoAsync();
        var newFriend = new SteamFriendInfo();
        var newFriendObj = Instantiate(friendObj, friendListContentPanel.transform);
        newFriend.nameText = newFriendObj.GetComponentInChildren<TMP_Text>();
        switch (status)
        {
            case 1:
                newFriend.nameText.text = "<color=#69FF69>" + friend.Name + "</color>";
                break;
            case 2:
                newFriend.nameText.text = "<color=#69D0FF>" + friend.Name + "</color>";
                break;
            case 3:
                newFriend.nameText.text = "<color=#505050>" + friend.Name + "</color>";
                break;
        }
        _friendList.Add(newFriend);
        friendButton = newFriendObj.GetComponent<FriendButton>();
        friendButton.SteamID = friend.Id;
        newFriend.avatar = newFriendObj.GetComponentInChildren<RawImage>();
        var imageData = await SteamFriends.GetSmallAvatarAsync(friend.Id);
        var newImage = new Texture2D((int)imageData.Value.Width, (int)imageData.Value.Height, TextureFormat.RGBA32, false, false);
        newImage.LoadRawTextureData(imageData.Value.Data);
        newImage.Apply();
        newFriend.avatar.texture = newImage;
    }
}
