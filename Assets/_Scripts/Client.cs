using UnityEngine;
using Colyseus;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Client : MonoBehaviour 
{
    [Header("Connection Settings")]
    [SerializeField] private string serverUrl = "ws://192.168.1.131:2567";
    [SerializeField] private string roomName = "level_room";

    [Header("UI")]
    [SerializeField] private ChatUIManager chatUIManager;

    private ColyseusClient client;
    private ColyseusRoom<LevelState> levelRoom;
    private bool isConnected = false;

    public event Action<string, string> OnChatMessageReceived;
    public event Action<string> OnWelcomeMessageReceived;
    public event Action<bool> OnConnectionStatusChanged;

    private async void Start()
    {
        await InitializeClient();
        await JoinRoom();
    }

    private async Task InitializeClient()
    {
        try
        {
            if (string.IsNullOrEmpty(serverUrl) || !serverUrl.StartsWith("ws://"))
                throw new Exception("Invalid WebSocket URL");

            client = new ColyseusClient(serverUrl);
            Debug.Log($"Colyseus client initialized: {serverUrl}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Init failed: {e.Message}");
        }
    }

    public async Task<bool> JoinRoom()
    {
        if (client == null)
        {
            Debug.LogError("Client not initialized");
            return false;
        }

        try
        {
            levelRoom = await client.Join<LevelState>(roomName);
            SetupRoomListeners();
            isConnected = true;
            OnConnectionStatusChanged?.Invoke(true);
            Debug.Log("Joined room successfully");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Join failed: {e.Message}");
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            return false;
        }
    }

    public void SetupRoomListeners()
    {
        if (levelRoom == null) return;

        levelRoom.OnMessage<LevelState>("initData", (data) =>
        {
            Debug.Log("Level: " + data.levelName);
            chatUIManager.SetMessages(data.messages, data.levelName);
        });
        levelRoom.OnMessage<Message>("newMessage", (newMsg) =>
        {
            chatUIManager.AddNewMessage(newMsg);
        });

        levelRoom.OnMessage<Message>("messageUpdated", (updatedMsg) =>
        {
            chatUIManager.UpdateMessage(updatedMsg);
        });
        levelRoom.OnMessage<ChatMessage>("chat", (chatMessage) =>
        {
            string sender = chatMessage.sender ?? "Unknown";
            OnChatMessageReceived?.Invoke(sender, chatMessage.message);
        });

        levelRoom.OnMessage<string>("welcomeMessage", (message) =>
        {
            OnWelcomeMessageReceived?.Invoke(message);
        });

        levelRoom.OnLeave += (code) =>
        {
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            Debug.Log($"Disconnected from room. Code: {code}");
        };

    }

    public async Task SendChatMessage(string messageContent , string parentId = null)
    {
        if (!isConnected || levelRoom == null)
        {
            Debug.LogWarning("Not connected to room.");
            return;
        }

        Message chat = new Message
        {
            
            content = messageContent,
            votes = 0,
            parentId = parentId 
        };

        try
        {
            await levelRoom.Send("sendMessage", chat);
            levelRoom.OnMessage<LevelState>("initData", (data) =>
            {
                Debug.Log("Level: " + data.levelName);
                chatUIManager.SetMessages(data.messages, data.levelName);
            });
            Debug.Log("Message sent.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Send failed: {e.Message}");
        }
    }


    public async Task SendVote(string messageId, bool isUpvote)
    {
        if (!isConnected || levelRoom == null)
        {
            Debug.LogWarning("Not connected to room.");
            return;
        }

        try
        {
            var voteData = new Dictionary<string, object>
        {
            { "id", messageId },
            { "isUpvote", isUpvote } 
        };

            await levelRoom.Send("vote", voteData);
            Debug.Log($"VOTE sent: {(isUpvote ? "UP" : "DOWN")} for message ID: {messageId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SendVote failed: {e.Message}");
        }
    }

}

