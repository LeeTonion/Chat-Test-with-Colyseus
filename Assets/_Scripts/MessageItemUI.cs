using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageItemUI : MonoBehaviour
{
    [SerializeField] public TMP_Text senderText;
    [SerializeField] public TMP_Text contentText;
    [SerializeField] public TMP_Text timestampText;
    [SerializeField] public TMP_Text levelNameText;
    [SerializeField] public Button UpVote;
    [SerializeField] public Button DownVote;
    [SerializeField] public TextMeshProUGUI VoteCount;
    [SerializeField] public Button repliesButton;
    [SerializeField] public TextMeshProUGUI RepliesCount;
    public Button replyButton;
    public string messageId;
    public bool _isItem = false;
    public void Setup(Message msg, string levelName)  
    {
        messageId = msg.id;
        senderText.text =msg.sender;
        contentText.text = msg.content;
        timestampText.text = UnixTimeToString(msg.timestamp);
        levelNameText.text = $"LV{levelName}";
        VoteCount.text = msg.votes.ToString();
    }

    private string UnixTimeToString(long timestamp)
    {
        try
        {
            if (timestamp > 9999999999)
            {
                timestamp = timestamp / 1000;
            }

            System.DateTime dateTime = System.DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            return dateTime.ToString("HH:mm:ss dd/MM/yyyy");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Invalid timestamp: {timestamp}, Error: {ex.Message}");
            return "Invalid Date";
        }
    }
}
