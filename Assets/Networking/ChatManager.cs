using Steamworks;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class ChatManager : MonoBehaviour {
    
    public SteamManager steamManager;
    private CinemachineFreeLook _cameraFreeLook;
    private CameraControls _cameraControls;
    private CharacterLocomotion _characterLocomotion;
    public TMP_InputField chatInputField;
    public GameObject cinemachineCamera, chatCanvas, characterObj, scrollViewObj;
    public GameObject contentPanel, chatInputFieldObject, textObject;
    private CanvasGroup _canvasGroup;
    private int _maxMessages;
    public bool isChatWindowOpen;
    private readonly List<Message> _messageList = new List<Message>();
    private class Message {
        public int nameLength, overflow;
        public string timestamp, name, text;
        public TMP_Text textText;
    }

    private void Start(){
        steamManager = SteamManager.instance;
        _cameraFreeLook = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        _canvasGroup = scrollViewObj.GetComponent<CanvasGroup>();
        _cameraControls = cinemachineCamera.GetComponent<CameraControls>();
        _characterLocomotion = characterObj.GetComponent<CharacterLocomotion>();
        _maxMessages = 10;
        isChatWindowOpen = false;
        SelfJoinMessage();
    }

    private void SelfJoinMessage() {
        const string messageIdentifier = "o";
        var messageTimeStamp = DateTime.Now.ToString("HH:mm");
        var messageName = SteamClient.Name;
        var encodedMessage = messageIdentifier + messageTimeStamp + messageName;
        var messageToByte = Encoding.UTF8.GetBytes(encodedMessage);
        JoinedChatMessage(messageToByte);
        steamManager.SendMessageToSocketServer(messageToByte);
    }

    private void Update(){
        if ( Input.GetKeyDown(KeyCode.Return) ) {
            if ( !chatCanvas.activeSelf || !chatInputFieldObject.activeSelf ) {
                StopAllCoroutines();
                OpenChatWindow();
            }
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
        if ( Input.GetKeyDown(KeyCode.Escape) && chatCanvas.activeSelf && chatInputFieldObject.activeSelf ) CloseChatWindow();
    }

    private void OpenChatWindow() {
        _canvasGroup.alpha = 1;
        chatCanvas.SetActive(true);
        chatInputFieldObject.SetActive(true);
        chatInputField.ActivateInputField();
        Cursor.lockState = CursorLockMode.None;
        _cameraFreeLook.m_YAxis.m_MaxSpeed = 0f;
        _cameraFreeLook.m_XAxis.m_MaxSpeed = 0f;
        _cameraControls.enabled = false;
        _characterLocomotion.enabled = false;
        isChatWindowOpen = true;
    }

    private void CloseChatWindow() {
        chatInputFieldObject.SetActive(false);
        chatCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        _cameraFreeLook.m_YAxis.m_MaxSpeed = 0.01f;
        _cameraFreeLook.m_XAxis.m_MaxSpeed = 1f;
        _cameraControls.enabled = true;
        _characterLocomotion.enabled = true;
    }

    private void SendChatMessage(string chatText) {
        const string messageIdentifier = "n";
        var messageTimeStamp = DateTime.Now.ToString("HH:mm");
        var messageName = SteamClient.Name;
        var messageNameLength = messageName.Length;
        var messageNameOverflow = 0;
        if (messageNameLength > 9) messageNameOverflow = 1;
        var encodedMessage = messageIdentifier+messageNameOverflow+messageNameLength+messageTimeStamp+messageName+chatText;
        var messageToByte = Encoding.UTF8.GetBytes(encodedMessage);
        steamManager.SendMessageToSocketServer(messageToByte);
        ReceiveChatMessage(messageToByte);
    }

    public void ReceiveChatMessage(byte[] eMessage) {
        if ( _messageList.Count >= _maxMessages ) {
            Destroy(_messageList[0].textText.gameObject);
            _messageList.Remove(_messageList[0]);
        }

        var newMessage = new Message();
        newMessage.overflow = int.Parse(Encoding.UTF8.GetString(eMessage, 1, 1));
        newMessage.nameLength = int.Parse(Encoding.UTF8.GetString(eMessage, 2, 1+newMessage.overflow));
        newMessage.timestamp = Encoding.UTF8.GetString(eMessage, 3+newMessage.overflow, 5);
        newMessage.name = Encoding.UTF8.GetString(eMessage, 8+newMessage.overflow, newMessage.nameLength);
        var textStartPos = 8 + newMessage.overflow + newMessage.nameLength;
        newMessage.text = Encoding.UTF8.GetString(eMessage, textStartPos, eMessage.Length-textStartPos);
        var newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <color=#00FFFF>"+
                                    newMessage.name+"</color>: "+newMessage.text;
        _messageList.Add(newMessage);

        if ( !chatCanvas.activeSelf ) StartCoroutine( ChatFadeOut() );
    }

    private IEnumerator ChatFadeOut() {
        chatCanvas.SetActive(true);
        chatInputFieldObject.SetActive(false);
        var initialTimer = 1f;
        while (initialTimer > 0 ) {
            initialTimer -= Time.deltaTime/5;
            yield return null;
        }
        while ( _canvasGroup.alpha > 0 ) {
            _canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        chatInputFieldObject.SetActive(true);
        chatCanvas.SetActive(false);
        _canvasGroup.alpha = 1;
        yield return null;
    }

    public void JoinedChatMessage(byte[] eMessage) {
        if ( _messageList.Count >= _maxMessages ) {
            Destroy(_messageList[0].textText.gameObject);
            _messageList.Remove(_messageList[0]);
        }
        var newMessage = new Message();
        newMessage.timestamp = Encoding.UTF8.GetString(eMessage, 1, 5);
        newMessage.name = Encoding.UTF8.GetString(eMessage, 6, eMessage.Length-6);
        var newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <color=#FFFF00>"+
                                    newMessage.name+" has joined the world!</color>";
        _messageList.Add(newMessage);

        if ( !chatCanvas.activeSelf ) StartCoroutine( ChatFadeOut() );
    }
}