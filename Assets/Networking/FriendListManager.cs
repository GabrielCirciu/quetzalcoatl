using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendListManager : MonoBehaviour {
    
    int friendListLength = 0;
    public GameObject friendListContentPanel, friendObj;
    [SerializeField]
    List<SteamFriendInfo> friendList = new List<SteamFriendInfo>();
    [Serializable]
    public class SteamFriendInfo {
        public TMP_Text nameText;
        public RawImage avatar;
    }
    public FriendButton friendButton;

    public void GetSteamFriends() {
        if ( friendListLength != 0 ) {
            for ( int i = 0; i < friendListLength; i++ ) {
                Destroy(friendList[0].nameText.transform.parent.gameObject);
                friendList.Remove(friendList[0]);
            }
            friendListLength = 0;
        }
        GetPlayingFriends();
        GetOnlineFriends();
        GetOfflineFriends();
    }

    void GetPlayingFriends() {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if ( friend.IsPlayingThisGame ) PopulateFriendList(friend, 1);  
        }
    }

    void GetOnlineFriends() {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if ( friend.IsOnline && !friend.IsPlayingThisGame ) PopulateFriendList(friend, 2);
        }
    }

    void GetOfflineFriends() {
        foreach ( var friend in SteamFriends.GetFriends() ) {
            if ( !friend.IsOnline ) PopulateFriendList(friend, 3);
        }
    }

    async void PopulateFriendList(Friend friend, int status) {
        friendListLength++;
        await friend.RequestInfoAsync();
        SteamFriendInfo newFriend = new SteamFriendInfo();
        GameObject newFriendObj = Instantiate(friendObj, friendListContentPanel.transform);
        newFriend.nameText = newFriendObj.GetComponentInChildren<TMP_Text>();
        if ( status == 1 ) newFriend.nameText.text = "<color=#69FF69>"+friend.Name+"</color>";
        else if ( status == 2 ) newFriend.nameText.text = "<color=#69D0FF>"+friend.Name+"</color>";
        else if ( status == 3 ) newFriend.nameText.text = "<color=#696969>"+friend.Name+"</color>";
        friendList.Add(newFriend);
        friendButton = newFriendObj.GetComponent<FriendButton>();
        friendButton.steamID = friend.Id;
        newFriend.avatar = newFriendObj.GetComponentInChildren<RawImage>();
        var imageData = await SteamFriends.GetSmallAvatarAsync(friend.Id);
        Texture2D newImage = new Texture2D((int)imageData.Value.Width, (int)imageData.Value.Height, TextureFormat.RGBA32, false, false);
        newImage.LoadRawTextureData(imageData.Value.Data);
        newImage.Apply();
        newFriend.avatar.texture = newImage;
    }
}
