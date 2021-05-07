using Steamworks;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class ChatManager : MonoBehaviour {
    public SteamManager steamManager;
    CinemachineFreeLook cameraFreeLook;
    public TMP_InputField chatInputField;
    public GameObject cinemachineCamera, chatCanvas, characterObj;
    public GameObject contentPanel, chatInputFieldObject, textObject;
    int maxMessages;
    public bool isChatWindowOpen;
    public List<Message> messageList = new List<Message>();
    public class Message {
        public int namelength, overflow;
        public string identifier, timestamp, name, text;
        public TMP_Text textText;
    }

    void Start(){
        steamManager = GameObject.Find("SteamManager").GetComponent<SteamManager>();
        cameraFreeLook = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        maxMessages = 10;
        isChatWindowOpen = false;
    }

    void Update(){
        if ( Input.GetKeyDown(KeyCode.Return) ) {
            if ( !chatCanvas.activeSelf ) OpenChatWindow();
            else {
                if ( chatInputField.text == "" ) {
                    CloseChatWindow();
                    isChatWindowOpen = false;
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
        if ( Input.GetKeyDown(KeyCode.Escape) && chatCanvas.activeSelf ) CloseChatWindow();
    }

    void OpenChatWindow() {
        chatCanvas.SetActive(true);
        chatInputFieldObject.SetActive(true);
        chatInputField.ActivateInputField();
        Cursor.lockState = CursorLockMode.None;
        cameraFreeLook.m_YAxis.m_MaxSpeed = 0f;
        cameraFreeLook.m_XAxis.m_MaxSpeed = 0f;
        cinemachineCamera.GetComponent<CameraControls>().enabled = false;
        characterObj.GetComponent<CharacterLocomotion>().enabled = false;
        isChatWindowOpen = true;
    }

    void CloseChatWindow() {
        chatInputFieldObject.SetActive(false);
        chatCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        cameraFreeLook.m_YAxis.m_MaxSpeed = 3f;
        cameraFreeLook.m_XAxis.m_MaxSpeed = 1f;
        cinemachineCamera.GetComponent<CameraControls>().enabled = true;
        characterObj.GetComponent<CharacterLocomotion>().enabled = true;
    }

    public void SendChatMessage(string chatText) {
        string messageIdentifier = "n";
        string messageTimeStamp = DateTime.Now.ToString("HH:mm");
        string messageName = SteamClient.Name.ToString();
        int messageNameLength = messageName.Length;
        int messageNameOverflow = 0;
        if (messageNameLength > 9) messageNameOverflow = 1;
        string encodedMessage = messageIdentifier+messageNameOverflow+messageNameLength.ToString()+messageTimeStamp+messageName+chatText;
        byte[] messageToByte = System.Text.Encoding.UTF8.GetBytes(encodedMessage);
        steamManager.SendMessageToSocketServer(messageToByte);
        RecieveChatMessage(messageToByte);
    }

    public void RecieveChatMessage(byte[] eMessage) {
        if ( messageList.Count >= maxMessages ) {
            Destroy(messageList[0].textText.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.overflow = int.Parse(System.Text.Encoding.UTF8.GetString(eMessage, 1, 1));
        newMessage.namelength = int.Parse(System.Text.Encoding.UTF8.GetString(eMessage, 2, 1+newMessage.overflow));
        newMessage.timestamp = System.Text.Encoding.UTF8.GetString(eMessage, 3+newMessage.overflow, 5);
        newMessage.name = System.Text.Encoding.UTF8.GetString(eMessage, 8+newMessage.overflow, newMessage.namelength);
        int textStartPos = 8 + newMessage.overflow + newMessage.namelength;
        newMessage.text = System.Text.Encoding.UTF8.GetString(eMessage, textStartPos, eMessage.Length-textStartPos);
        GameObject newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <b>[</b> <color=#00FFFF>"+
                                    newMessage.name+"</color> <b>]</b>: "+newMessage.text;
        messageList.Add(newMessage);
    }

    public void JoinedChatMessage(byte[] eMessage) {
        Debug.Log("We should have recieved a message");
        if ( messageList.Count >= maxMessages ) {
            Destroy(messageList[0].textText.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.overflow = int.Parse(System.Text.Encoding.UTF8.GetString(eMessage, 1, 1));
        newMessage.namelength = int.Parse(System.Text.Encoding.UTF8.GetString(eMessage, 2, 1+newMessage.overflow));
        newMessage.timestamp = System.Text.Encoding.UTF8.GetString(eMessage, 3+newMessage.overflow, 5);
        newMessage.name = System.Text.Encoding.UTF8.GetString(eMessage, 8+newMessage.overflow, newMessage.namelength);
        int textStartPos = 8 + newMessage.overflow + newMessage.namelength;
        newMessage.text = System.Text.Encoding.UTF8.GetString(eMessage, textStartPos, eMessage.Length-textStartPos);
        GameObject newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <b><color=#FFFF00>"+
                                    newMessage.name+"</color></b>: "+newMessage.text;
        messageList.Add(newMessage);
    }
}