using UnityEngine;
using Colyseus;
using System.Threading.Tasks;
using System;

public class Client : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private string serverUrl = "ws://192.168.1.135:2567"; // Đã đúng IP
    [SerializeField] private string roomName = "my_room";

    private ColyseusClient client;
    private ColyseusRoom<MyRoomState> room;
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
            // Kiểm tra URL hợp lệ trước khi khởi tạo
            if (string.IsNullOrEmpty(serverUrl) || !serverUrl.StartsWith("ws://"))
            {
                throw new Exception("Invalid WebSocket URL");
            }

            client = new ColyseusClient(serverUrl);
            Debug.Log($"Colyseus client initialized with URL: {serverUrl}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize client: {e.Message}");
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
            room = await client.Join<MyRoomState>(roomName);
            SetupRoomListeners();
            isConnected = true;
            OnConnectionStatusChanged?.Invoke(true);
            Debug.Log($"Successfully joined room: {roomName} at {serverUrl}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join room: {e.Message}");
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            return false;
        }
    }

    private void SetupRoomListeners()
    {
        if (room == null) return;

        room.OnMessage<ChatMessage>("chat", (chatMessage) =>
        {
            string sender = chatMessage.sender ?? "Unknown";
            OnChatMessageReceived?.Invoke(sender, chatMessage.message);
        });

        room.OnMessage<string>("welcomeMessage", (message) =>
        {
            OnWelcomeMessageReceived?.Invoke(message);
        });

        room.OnLeave += (code) =>
        {
            isConnected = false;
            OnConnectionStatusChanged?.Invoke(false);
            Debug.Log($"Disconnected from room with code: {code}");
        };
    }

    public async Task SendChatMessage(string message)
    {
        if (!isConnected || room == null)
        {
            Debug.LogWarning("Cannot send message: Not connected to room");
            return;
        }

        try
        {
            if (!string.IsNullOrEmpty(message))
            {
                await room.Send("chat", message);
                Debug.Log($"Message sent: {message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send message: {e.Message}");
        }
    }

    private async void OnDestroy()
    {
        if (room != null && isConnected)
        {
            try
            {
                await room.Leave();
                Debug.Log("Room left successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error leaving room: {e.Message}");
            }
        }
    }

    public async void CreateRoom()
    {
        if (client == null)
        {
            Debug.LogError("Client not initialized");
            return;
        }

        try
        {
            room = await client.Create<MyRoomState>("my_room");
            Debug.Log($"Room created successfully at {serverUrl}");

            room.OnMessage<ChatMessage>("chat", (chatMessage) =>
            {
                Debug.Log($"[{chatMessage.sender}]: {chatMessage.message}");
            });

            room.OnMessage<string>("welcomeMessage", (message) =>
            {
                Debug.Log(message);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create room: {e.Message}");
        }
    }
}