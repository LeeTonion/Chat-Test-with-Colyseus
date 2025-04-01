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
    public Client networkClient; // Tham chiếu tới Client

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
        string timeStamp = DateTime.Now.ToString("HH:mm:ss");
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

        AdjustMessageHeight(messageText);
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
    public async void OnGifButtonClicked(String GifMess)
    {
        string gifMessage = GifMess;
        await networkClient.SendChatMessage(gifMessage);
        chatInput.text = chatInput.text.Replace(gifMessage, "");
        EmojiManager.Instance.HideIconAndGifButton();
        chatInput.ActivateInputField();
        Gif = true;
    }
    /*  private void Start()
      {
          Instance = this;
          sendButton.onClick.AddListener(OnSendButtonClicked);
          toggleChatButton.onClick.AddListener(ToggleChatVisibility);
      }

      private void OnSendButtonClicked()
      {
          string message = chatInput.text.Trim();

          if (!string.IsNullOrWhiteSpace(message))
          {
              // Gọi hàm hiển thị tin nhắn trong ứng dụng
              DisplayMessage(message);
              chatInput.text = "";
              chatInput.ActivateInputField();
          }
      }

      private void DisplayMessage(string message)
      {
          GameObject newMessageObj;

          // Kiểm tra xem tin nhắn có phải là GIF không (dựa vào từ khóa hoặc định dạng của tin nhắn)
          if (message.Contains("[GIF]"))
          {
              newMessageObj = Instantiate(gifMessagePrefab, content);
              message = message.Replace("[GIF]", "");  // Loại bỏ tag [GIF] nếu có
          }
          else
          {
              newMessageObj = Instantiate(messagePrefab, content);
          }

          TextMeshProUGUI messageText = newMessageObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
          TextMeshProUGUI NameText = newMessageObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

          messageText.text = message;
          NameText.text = "You :";

          AdjustMessageHeight(messageText);
          messageCount++;
          UpdateMessageCount();

          Canvas.ForceUpdateCanvases();
          LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
          scrollRect.verticalNormalizedPosition = 0f;
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

      // Hàm khi click vào nút GIF để gửi tin nhắn GIF
      public async void OnGifButtonClicked(String GifMess)
      {
          // Thêm [GIF] vào tin nhắn để nhận diện là GIF
          string gifMessage = "[GIF]" + GifMess;

          // Gọi hàm hiển thị tin nhắn GIF trong ứng dụng
          DisplayMessage(gifMessage);
          chatInput.text = chatInput.text.Replace(gifMessage, "");
          EmojiManager.Instance.HideIconAndGifButton();
          chatInput.ActivateInputField();
          Gif = true;
      }*/
}