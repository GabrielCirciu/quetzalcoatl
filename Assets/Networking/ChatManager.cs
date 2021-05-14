using System;
using Steamworks;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class ChatManager : MonoBehaviour {
    
    public static ChatManager instance;
    private SteamManager _steamManager;
    private CinemachineFreeLook _cameraFreeLook;
    private CameraControls _cameraControls;
    private CharacterLocomotion _characterLocomotion;
    public TMP_InputField chatInputField;
    public GameObject cinemachineCamera, chatCanvas, characterObj, scrollViewObj;
    public GameObject contentPanel, chatInputFieldObject, textObject;
    private CanvasGroup _canvasGroup;
    private const int MAXMessages = 10;
    public bool isChatWindowOpen;
    private readonly List<Message> _messageList = new List<Message>();
    private class Message
    {
        public int nameLength;
        public string timestamp, name, text;
        public TMP_Text textText;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _steamManager = SteamManager.instance;
        _cameraFreeLook = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        _canvasGroup = scrollViewObj.GetComponent<CanvasGroup>();
        _cameraControls = cinemachineCamera.GetComponent<CameraControls>();
        _characterLocomotion = characterObj.GetComponent<CharacterLocomotion>();
        isChatWindowOpen = false;
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

    private void RemoveOldChat()
    {
        if ( _messageList.Count >= MAXMessages )
        {
            Destroy(_messageList[0].textText.gameObject);
            _messageList.Remove(_messageList[0]);
        }
    }
    
    private void SendChatMessage(string chatText)
    {
        // # - Do not save, n - Chat message
        const string messageIdentifier = "#n";
        var messageName = SteamClient.Name;
        var messageNameLength = (char)messageName.Length;
        var messageString = messageIdentifier+messageNameLength+messageName+chatText;
        var messageToByte = Encoding.UTF8.GetBytes(messageString);
        _steamManager.SendMessageToSocketServer(messageToByte);
        ReceiveChatMessage(messageToByte);
    }

    public void ReceiveChatMessage(byte[] eMessage)
    {
        RemoveOldChat();
        
        var newMessage = new Message();
        newMessage.timestamp = DateTime.Now.ToString("HH:m");
        newMessage.nameLength = eMessage[2];
        newMessage.name = Encoding.UTF8.GetString(eMessage, 3, newMessage.nameLength);
        var textStartPos = 3 + newMessage.nameLength;
        newMessage.text = Encoding.UTF8.GetString(eMessage, textStartPos, eMessage.Length-textStartPos);
        var newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <color=#00FFFF>"+
                                    newMessage.name+"</color>: "+newMessage.text;
        
        _messageList.Add(newMessage);

        if ( !chatCanvas.activeSelf ) StartCoroutine( ChatFadeOut() );
    }
    
    public void ReceiveJoinedMessage(byte[] dataArray)
    {
        RemoveOldChat();
        
        var newMessage = new Message();
        newMessage.timestamp = DateTime.Now.ToString("HH:m");
        var startingPos = 3 + dataArray[2];
        newMessage.name = Encoding.UTF8.GetString(dataArray, startingPos, dataArray.Length-startingPos);
        var newTextObject = Instantiate(textObject, contentPanel.transform);
        newMessage.textText = newTextObject.GetComponent<TMP_Text>();
        newMessage.textText.text = "<size=10><color=#FF9600>"+newMessage.timestamp+"</color></size> <color=#FFFF00>"+
                                    newMessage.name+" has joined the world!</color>";
        
        _messageList.Add(newMessage);

        if ( !chatCanvas.activeSelf ) StartCoroutine( ChatFadeOut() );
    }
}