using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

public class ChatManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField chatInput;
    public Button sendButton;
    public Transform content;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;
    public Button toggleChatButton;
    public TextMeshProUGUI messageCountText;
    public GameObject chatPanel;

    [Header("Network")]
    public Client networkClient; // Tham chiếu tới Client

    private int messageCount = 0;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked);
        chatInput.lineType = TMP_InputField.LineType.MultiLineNewline;
        toggleChatButton.onClick.AddListener(ToggleChatVisibility);

        // Đăng ký sự kiện nhận tin nhắn từ server
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
        string timeStamp = DateTime.Now.ToString("HH:mm:ss");
        GameObject newMessageObj = Instantiate(messagePrefab, content);

        TextMeshProUGUI messageText = newMessageObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI NameText = newMessageObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        messageText.text = $"{message}               ({timeStamp})";
             
        NameText.text = $"{sender} :";

        AdjustMessageHeight(messageText);
        messageCount++;
        UpdateMessageCount();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0f;
    }

    

    private void OnConnectionStatusChanged(bool isConnected)
    {
        chatInput.interactable = isConnected;
        sendButton.interactable = isConnected;
        Debug.Log($"Connection status: {(isConnected ? "Connected" : "Disconnected")}");
    }

    private void AdjustMessageHeight(TextMeshProUGUI messageText)
    {
        RectTransform messageRect = messageText.GetComponent<RectTransform>();
        messageRect.sizeDelta = new Vector2(messageRect.sizeDelta.x, messageText.preferredHeight);
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