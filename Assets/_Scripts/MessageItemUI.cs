using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text timestampText;
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] public Button upVoteButton;
    [SerializeField] public Button downVoteButton;
    [SerializeField] public TextMeshProUGUI CountlikesText;
    [SerializeField] public Image voteImage;
    [SerializeField] public Button repliesButton;
    [SerializeField] private TextMeshProUGUI repliesCountText;
    [SerializeField] public Button replyButton;
    [SerializeField] private RectTransform rectTransform;

    public double CommentId { get; private set; }
    public bool IsItem { get; set; }

    public void Setup(CommentSchema comment, string levelName, float indent = 0f)
    {
        CommentId = comment.id;
        senderText.text = string.IsNullOrEmpty(comment.account.name) ? "Unknown" : comment.account.name;
        contentText.text = comment.detail;
       // timestampText.text = GetTimestampText(comment.createdAt, comment.updatedAt);
        levelNameText.text = string.IsNullOrEmpty(levelName) ? "" : $"LV{levelName}";


        CountlikesText.text = (comment.likes-comment.dislikes).ToString();

        repliesCountText.text = comment.replies.ToString();
        rectTransform.anchoredPosition += new Vector2(indent, 0);
    }

    public void UpdateComment(CommentSchema comment)
    {
        contentText.text = comment.detail;
     //  timestampText.text = GetTimestampText(comment.createdAt, comment.updatedAt);
        CountlikesText.text = (comment.likes - comment.dislikes).ToString();
        repliesCountText.text = comment.replies.ToString();
    }

    private string GetTimestampText(long createdAt, long updatedAt)
    {
        try
        {
            long timestamp = createdAt;
            if (timestamp > 9999999999) // Xử lý timestamp millisecond
            {
                timestamp /= 1000;
            }

            System.DateTime dateTime = System.DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            string dateString = dateTime.ToString("HH:mm:ss dd/MM/yyyy");

            if (updatedAt > createdAt + 1000) // Bỏ qua chênh lệch nhỏ
            {
                dateString += " (đã chỉnh sửa)";
            }

            return dateString;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Invalid timestamp: {createdAt}, Error: {ex.Message}");
            return "Invalid Date";
        }
    }
}