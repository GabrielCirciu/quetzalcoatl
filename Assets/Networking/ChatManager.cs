using Steamworks;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    Encoding encUTF8 = Encoding.UTF8;
    public SteamManager steamManager;
    public InputField chatInputField;
    public GameObject chatPanel, contentPanel, chatInputFieldObject, playerControllerObject, textObject;
    PlayerMovement playerMovement;
    public int maxMessages;
    public List<Message> messageList = new List<Message>();
    public class Message {
        public int namelength, overflow;
        public string identifier, timestamp, name, text;
        public Text textText;
    }

    void Start(){
        steamManager = GameObject.Find("SteamManagerGameObject").GetComponent<SteamManager>();
    }

    void Update(){
        if ( Input.GetKeyDown(KeyCode.Return) ) {
            if ( !chatPanel.activeSelf ) {
                chatPanel.SetActive(true);
                chatInputFieldObject.SetActive(true);
                chatInputField.ActivateInputField();
                playerControllerObject.GetComponent<PlayerMovement>().enabled = false;
                playerControllerObject.GetComponentInChildren<LookWithMouse>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                if ( chatInputField.text == "" ) {
                    chatInputFieldObject.SetActive(false);
                    chatPanel.SetActive(false);
                    playerControllerObject.GetComponent<PlayerMovement>().enabled = true;
                    playerControllerObject.GetComponentInChildren<LookWithMouse>().enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else {
                    SendChatMessage(chatInputField.text);
                    chatInputField.text = "";
                    chatInputFieldObject.SetActive(false);
                    chatInputFieldObject.SetActive(true);
                    chatInputField.ActivateInputField();
                }
            }
        }
    }

    public void SendChatMessage(string chatText) {
        string messageIdentifier = "n";
        string messageTimeStamp = DateTime.Now.ToString("HH:mm");
        string messageName = SteamClient.Name.ToString();
        int messageNameLength = messageName.Length;
        int messageNameOverflow = 0;
        if (messageNameLength > 9) { messageNameOverflow = 1; }
        string encodedMessage = messageIdentifier+messageNameOverflow+messageNameLength.ToString()+messageTimeStamp+messageName+chatText;
        byte[] messageToByte = encUTF8.GetBytes(encodedMessage);
        steamManager.SendMessageToSocketServer(messageToByte);
        RecieveChatMessage(messageToByte);
    }

    public void RecieveChatMessage(byte[] eMessage) {
        if ( messageList.Count >= maxMessages ) {
            Destroy(messageList[0].textText.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.overflow = int.Parse(encUTF8.GetString(eMessage, 1, 1));
        newMessage.namelength = int.Parse(encUTF8.GetString(eMessage, 2, 1+newMessage.overflow));
        newMessage.timestamp = encUTF8.GetString(eMessage, 3+newMessage.overflow, 5);
        newMessage.name = encUTF8.GetString(eMessage, 8+newMessage.overflow, newMessage.namelength);
        int textStartPos = 8 + newMessage.overflow + newMessage.namelength;
        newMessage.text = encUTF8.GetString(eMessage, textStartPos, eMessage.Length-textStartPos);
        GameObject newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<Text>();
        newMessage.textText.text = " <size=12><color=orange>"+newMessage.timestamp+"</color></size> <b>[</b> <color=cyan>"+
                                    newMessage.name+"</color> <b>]</b>: "+newMessage.text;
        messageList.Add(newMessage);
    }
}