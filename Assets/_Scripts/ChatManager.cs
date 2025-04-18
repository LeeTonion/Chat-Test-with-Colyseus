using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class ChatManager : MonoBehaviour
{
   public static ChatManager Instance { get; private set; }
    [Header("UI Components")]
    public TMP_InputField chatInput;
    public Button sendButton;
    public Transform content;
    public GameObject messagePrefab;
    public GameObject gifMessagePrefab;
    public ScrollRect scrollRect;
    public Button toggleChatButton;
    public TextMeshProUGUI messageCountText;
    public GameObject chatPanel;
    private bool Gif = false;
    [Header("Network")]
    public Client1 networkClient; // Tham chiếu tới Client

    private int messageCount = 0;

    private void Start()
    {
        Instance = this;
        sendButton.onClick.AddListener(OnSendButtonClicked);
        //chatInput.lineType = TMP_InputField.LineType.MultiLineNewline;
        toggleChatButton.onClick.AddListener(ToggleChatVisibility);
        networkClient.OnChatMessageReceived += OnChatMessageReceived;
        networkClient.OnConnectionStatusChanged += OnConnectionStatusChanged;

        UpdateMessageCount();
    }

    private async void OnSendButtonClicked()
    {
        string message = chatInput.text.Trim();

        if (!string.IsNullOrWhiteSpace(message))
        {
            await networkClient.SendChatMessage(message);
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }

    private void OnChatMessageReceived(string sender, string message)
    {
        GameObject newMessageObj;
        if (Gif)
        {
            newMessageObj = Instantiate(gifMessagePrefab, content);
            
        }
        else
        {
            newMessageObj = Instantiate(messagePrefab, content);

        }


        TextMeshProUGUI messageText = newMessageObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI NameText = newMessageObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        messageText.text = $"{message}";

        NameText.text = $"{sender} :";
        messageCount++;
        UpdateMessageCount();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0f;
        Gif = false;
    }



    private void OnConnectionStatusChanged(bool isConnected)
    {
        chatInput.interactable = isConnected;
        sendButton.interactable = isConnected;
        Debug.Log($"Connection status: {(isConnected ? "Connected" : "Disconnected")}");
    }


    private void ToggleChatVisibility()
    {

        bool isActive = chatPanel.gameObject.activeSelf;
        chatPanel.gameObject.SetActive(!isActive);
    }

    private void UpdateMessageCount()
    {
        messageCountText.text = messageCount.ToString();
    }
    
    private void OnDestroy()
    {

        networkClient.OnChatMessageReceived -= OnChatMessageReceived;
        networkClient.OnConnectionStatusChanged -= OnConnectionStatusChanged;
    }


}