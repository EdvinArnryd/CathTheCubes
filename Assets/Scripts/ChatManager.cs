using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] private Message messagePrefab;
    [SerializeField] private CanvasGroup content;
    [SerializeField] private TMP_InputField input;

    public string playerName;

    private void Awake()
    {
        ChatManager.Singleton = this; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(input.text, playerName);
            input.text = "";
        }
    }

    public void SendChatMessage(string message, string fromWho = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        string S = fromWho + " > " + message;
        SendChatMessageServerRpc(S);
    }

    void AddMessage(string msg)
    {
        Message CM = Instantiate(messagePrefab, content.transform);
        CM.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }
}
