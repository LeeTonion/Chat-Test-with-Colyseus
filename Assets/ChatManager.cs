using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField chatInput; // Ô nhập liệu
    public Button sendButton; // Nút gửi
    public Transform content; // Khu vực hiển thị tin nhắn
    public GameObject messagePrefab; // Prefab tin nhắn (chứa cả Text nội dung và thời gian)
    public ScrollRect scrollRect; // Scroll View
    public Button toggleChatButton; // Nút tắt/bật khung chat
    public TextMeshProUGUI messageCountText; // Hiển thị số lượng tin nhắn
    public GameObject chatPanel; // Khung chat
    private int messageCount = 0; // Biến đếm số lượng tin nhắn

    private void Start()
    {
        // Đăng ký sự kiện bấm nút gửi
        sendButton.onClick.AddListener(SendMessage);

        // Cho phép InputField tự động xuống dòng
        chatInput.lineType = TMP_InputField.LineType.MultiLineNewline;

        // Đăng ký sự kiện tắt/bật khung chat
        toggleChatButton.onClick.AddListener(ToggleChatVisibility);

        UpdateMessageCount();
    }

    public void SendMessage()
    {
        string message = chatInput.text;

        if (!string.IsNullOrWhiteSpace(message))
        {
            // Lấy thời gian hiện tại
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");

            // Tạo tin nhắn mới
            GameObject newMessageObj = Instantiate(messagePrefab, content);

            // Gán nội dung và thời gian
            TextMeshProUGUI messageText = newMessageObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = newMessageObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            messageText.text = message;
            timeText.text = timeStamp;

            // Điều chỉnh kích thước khung chứa tin nhắn dựa trên nội dung
            AdjustMessageHeight(messageText);

            // Làm mới layout và cuộn xuống
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            scrollRect.verticalNormalizedPosition = 0f;

            // Cập nhật số lượng tin nhắn
            messageCount++;
            UpdateMessageCount();

            // Xóa nội dung đã nhập
            chatInput.text = "";



            // Focus lại InputField
            EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
        }
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
}