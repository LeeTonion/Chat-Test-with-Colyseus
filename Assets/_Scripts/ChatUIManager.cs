
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChatUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform messageContainer;
    [SerializeField] private GameObject messageItemPrefab;
    [SerializeField] private GameObject relayMessageItemPrefab;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] public TMP_InputField chatInputField;
    [SerializeField] private Button sendMessageButton;
    [SerializeField] private GameObject FormReply;
    [SerializeField] private TextMeshProUGUI Comment;
    [Header("Network")]
    [SerializeField] private Client networkClient;

    public static ChatUIManager Instance { get; private set; }
    public GameObject currentReplyTarget = null;

    private readonly List<Message> messageList = new();
    private readonly List<List<GameObject>> replyObjects = new();
    private Dictionary<string, int> votedMessages = new();
    private Dictionary<string, bool> repliesVisibility = new();
    private int currentIndex = 0;
    private string currentLevelName = "";

    private void Start()
    {
        Comment.text = "Leave your review";
        Instance = this;
        sendMessageButton.onClick.AddListener(OnSendMessageButtonClick);

    }

    public void SetMessages(Dictionary<string, Message> messagesDict, string levelName)
    {
        currentLevelName = levelName;
        levelText.text = $"Room {levelName}";

        messageList.Clear();
        replyObjects.Clear();

        messageList.AddRange(messagesDict.Values);
        SortMessagesByVote(messageList);

        currentIndex = 0;
        ShowNextMessages();
    }

    private void ShowNextMessages()
    {
        for (int i = currentIndex; i < messageList.Count; i++)
        {
            var msg = messageList[i];
            GameObject msgGO = CreateMessageItem(msg,messageItemPrefab);
            AddVoteHandlers(msg, msgGO);

            var repliesUI = new List<GameObject>();
            if (msg.replies.Count > 0)
            {
                SortMessagesByVote(msg.replies);
                foreach (var reply in msg.replies)
                {
                    GameObject replyGO = CreateMessageItem(reply, relayMessageItemPrefab);
                    AddVoteHandlers(reply, replyGO);
                    replyGO.SetActive(repliesVisibility.TryGetValue(msg.id, out bool visible) && visible);
                    repliesUI.Add(replyGO);
                }
            }

            replyObjects.Add(repliesUI);
            var msgUI = msgGO.GetComponent<MessageItemUI>();

            msgUI.replyButton.onClick.AddListener(() =>
            {
                sendMessageButton.onClick.RemoveAllListeners();
                FormReply.SetActive(true);
                Comment.text = $"Replying to {msg.sender} ";
                sendMessageButton.onClick.AddListener(() =>
                {
                    if (msgUI != null)
                    {
                        OnSendRelayMessageButtonClick(msgUI.gameObject);
                    }
                });
            });

            msgUI.RepliesCount.text = msg.replies.Count.ToString();
            int capturedIndex = i;
            msgUI.repliesButton.onClick.AddListener(() => ToggleReplies(capturedIndex));
        }
    }

    private GameObject CreateMessageItem(Message msg, GameObject prefab)
    {
        GameObject item = Instantiate(prefab, messageContainer);
        item.GetComponent<MessageItemUI>().Setup(msg, currentLevelName);
        return item;
    }

    private void AddVoteHandlers(Message msg, GameObject go)
    {
        var itemUI = go.GetComponent<MessageItemUI>();

        void UpdateVoteUI()
        {
            Image upImg = itemUI.UpVote.GetComponent<Image>();
            Image downImg = itemUI.DownVote.GetComponent<Image>();
            Image frame = itemUI.UpVote.transform.parent.GetComponent<Image>();

            upImg.color = Color.white;
            downImg.color = Color.white;
            itemUI.VoteCount.color = itemUI._isItem ? Color.black : itemUI.VoteCount.color;

            if (votedMessages.TryGetValue(msg.id, out int vote))
            {
                if (vote == 1)
                {
                    upImg.color = Color.blue;
                    if (itemUI._isItem)
                    {
                        frame.color = Color.blue;
                        itemUI.VoteCount.color = Color.white;
                    }
                }
                else if (vote == -1)
                {
                    downImg.color = Color.red;
                    if (itemUI._isItem)
                    {
                        frame.color = Color.red;
                        itemUI.VoteCount.color = Color.white;
                    }
                }
            }
        }

        itemUI.UpVote.onClick.AddListener(() =>
        {
            int currentVote = votedMessages.ContainsKey(msg.id) ? votedMessages[msg.id] : 0;

            if (currentVote == 1)
            {
                // Bỏ upvote
                msg.votes--;
                networkClient.SendVote(msg.id, false);
                votedMessages.Remove(msg.id);
            }
            else
            {
                if (currentVote == -1)
                {
                    // Gỡ downvote trước
                    msg.votes++;
                    networkClient.SendVote(msg.id, true);
                }

                // Áp dụng upvote
                msg.votes++;
                networkClient.SendVote(msg.id, true);
                votedMessages[msg.id] = 1;
            }

            RedrawMessages();
        });

        itemUI.DownVote.onClick.AddListener(() =>
        {
            int currentVote = votedMessages.ContainsKey(msg.id) ? votedMessages[msg.id] : 0;

            if (currentVote == -1)
            {
                // Bỏ downvote
                msg.votes++;
                networkClient.SendVote(msg.id, true);
                votedMessages.Remove(msg.id);
            }
            else
            {
                if (currentVote == 1)
                {
                    // Gỡ upvote trước
                    msg.votes--;
                    networkClient.SendVote(msg.id, false);
                }

                // Áp dụng downvote
                msg.votes--;
                networkClient.SendVote(msg.id, false);
                votedMessages[msg.id] = -1;
            }

            RedrawMessages();
        });

        UpdateVoteUI();
    }

    private void ToggleReplies(int index)
    {
        if (index >= replyObjects.Count) return;

        bool isActive = replyObjects[index].Count > 0 && replyObjects[index][0].activeSelf;

        foreach (var reply in replyObjects[index])
        {
            reply.SetActive(!isActive);
        }

        repliesVisibility[messageList[index].id] = !isActive;
    }

    private void SortMessagesByVote(List<Message> list)
    {
        list.Sort((a, b) => b.votes.CompareTo(a.votes));
    }
    public void CloseComment()
    {
        FormReply.SetActive(false);
        sendMessageButton.onClick.RemoveAllListeners();
        sendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
        chatInputField.text = "";
        EmojiManager.Instance.KeyboardButton();
        Comment.text = "Leave your review";

    }
    private void RedrawMessages()
    {
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }

        SortMessagesByVote(messageList);
        currentIndex = 0;
        replyObjects.Clear();
        ShowNextMessages();
    }

    public void AddNewMessage(Message msg)
    {
        if (string.IsNullOrEmpty(msg.parentId))
        {
            messageList.Add(msg);
            SortMessagesByVote(messageList);
            RedrawMessages();
        }
        else
        {
            var parent = messageList.Find(m => m.id == msg.parentId);
            if (parent != null)
            {
                parent.replies.Add(msg);
                SortMessagesByVote(parent.replies);
                RedrawMessages();
            }
        }
    }

    public void UpdateMessage(Message updatedMsg)
    {
        if (string.IsNullOrEmpty(updatedMsg.parentId))
        {
            int index = messageList.FindIndex(m => m.id == updatedMsg.id);
            if (index >= 0)
            {
                messageList[index].votes = updatedMsg.votes;
                SortMessagesByVote(messageList);
                RedrawMessages();
            }
        }
        else
        {
            var parent = messageList.Find(m => m.id == updatedMsg.parentId);
            if (parent != null)
            {
                int replyIndex = parent.replies.FindIndex(r => r.id == updatedMsg.id);
                if (replyIndex >= 0)
                {
                    parent.replies[replyIndex].votes = updatedMsg.votes;
                    SortMessagesByVote(parent.replies);
                    RedrawMessages();
                }
            }
        }
    }

    private async void OnSendRelayMessageButtonClick(GameObject go)
    {
        Debug.Log("Send relay message button clicked");
        if (!string.IsNullOrEmpty(chatInputField.text))
        {
            string content = chatInputField.text;
            chatInputField.text = "";

            string parentId = go.GetComponent<MessageItemUI>().messageId;
            await networkClient.SendChatMessage(content, parentId);

            sendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
            EmojiManager.Instance.KeyboardButton();
            chatInputField.transform.parent.gameObject.SetActive(false);

        }
    }

    private async void OnSendMessageButtonClick()
    {
        if (!string.IsNullOrEmpty(chatInputField.text))
        {
            string content = chatInputField.text;
            chatInputField.text = "";
            await networkClient.SendChatMessage(content);
        }
        chatInputField.transform.parent.gameObject.SetActive(false);
        EmojiManager.Instance.KeyboardButton();
        Canvas.ForceUpdateCanvases();
        messageContainer.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = -1f;
        Canvas.ForceUpdateCanvases();

    }
    public void Show()
    {

        chatInputField.transform.parent.gameObject.SetActive(true);
        EmojiManager.Instance.KeyboardButton();
        chatInputField.ActivateInputField();
    }
    public void Close()
    {
        messageContainer.transform.parent.parent.parent.gameObject.SetActive(false);
    }
}