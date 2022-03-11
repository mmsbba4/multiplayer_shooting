using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatMessage : MonoBehaviour
{
    public static ChatMessage instance;
    public PlayerMovement my_player;
    private void Start()
    {
        instance = this;
        GameManager.instance.OnStartGame.AddListener(InitChat);
    }
    public InputField typeMessage;
    public Transform chatMessage;
    public GameObject TextObject;
    public Transform TextParent;
    public ScrollRect rect;
    public bool isOnMessage = false;
    public bool isTyping = false;
    public float showDelta = 0;
    public bool isJoinedRoom = false;
    void DisableChat()
    {
        isJoinedRoom = false;
    }
    void InitChat()
    {
        isJoinedRoom = true;
    }
    void Update()
    {
        
        if (showDelta > 0)
        {
            showDelta -= Time.deltaTime;
            isOnMessage = true;
        }
        else
        {
            isOnMessage = false;
        }
        UpdateState();
        if (!isJoinedRoom) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isTyping = !isTyping;
            if (isTyping == false && typeMessage.text.Length > 0)
            {
                SendFromMe(typeMessage.text);
                typeMessage.text = string.Empty;
            }
        }
    }
    public void SendMessage(string sender, string message)
    {
        showDelta = 2f;
        GameObject newMess = Instantiate(TextObject, TextParent);
        newMess.GetComponent<Text>().text = System.DateTime.Now.Hour  +":"+ System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + " : <color=green>" + sender + "</color> : " + message;
        Invoke("DownLessView",0.15f);
    }
    void SendFromMe(string message)
    {
        my_player.SendMessageFrom(message);
    }
    void DownLessView()
    {
        rect.verticalNormalizedPosition = 0;
    }
    void UpdateState()
    {
        
        if (isOnMessage || isTyping)
        {
            chatMessage.gameObject.SetActive(true);
            if (isTyping)
            {
                typeMessage.gameObject.SetActive(true);
                typeMessage.Select();
                typeMessage.ActivateInputField();
            }
            else
            {
                typeMessage.gameObject.SetActive(false);
            }
        }
        else
        {
            chatMessage.gameObject.SetActive(false);
        }
        
    }
}
