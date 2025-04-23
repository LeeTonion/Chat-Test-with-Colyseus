using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class ChatUIManager : MonoBehaviour
{
    [SerializeField] private Transform commentContainer;
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button Send;
    [SerializeField] private Client networkClient;
    [SerializeField] private GameObject FormComment;
    private Dictionary<long, MessageItemUI> commentViews = new Dictionary<long, MessageItemUI>();
    private Dictionary<long, int> votedMessages = new();


    private void Start()
    {
        Send.onClick.AddListener(SendComment);
    }

    public void AddComment(CommentSchema comment, string levelName = "", float indent = 10f)
    {
        if (commentViews.ContainsKey((comment.id)))
        {
            Debug.Log($"Comment {comment.id} đã tồn tại, bỏ qua thêm mới.");
            return;
        }

        Debug.Log($"Thêm comment mới với ID: {comment.id}");
        GameObject newGO = Instantiate(commentPrefab, commentContainer);
        MessageItemUI itemUI = newGO.GetComponent<MessageItemUI>();

        commentViews[comment.id] = itemUI;

        itemUI.upVoteButton.onClick.AddListener(async () =>
        {
            int currentVote = votedMessages.ContainsKey(comment.id) ? votedMessages[comment.id] : 0;

            if (currentVote == 1)
            {
                comment.likes--;
                await networkClient.SendVote(comment.id, "unlike");
                votedMessages.Remove(comment.id);
            }
            else
            {
                if (currentVote == -1)
                {
                    comment.dislikes--;
                    await networkClient.SendVote(comment.id, "undislike");
                }

                comment.likes++;
                await networkClient.SendVote(comment.id, "like");
                votedMessages[comment.id] = 1;
            }

            itemUI.UpdateComment(comment);
            UpdateVoteColor(comment, itemUI);
        });


        itemUI.downVoteButton.onClick.AddListener(async () =>
        {
            int currentVote = votedMessages.ContainsKey(comment.id) ? votedMessages[comment.id] : 0;

            if (currentVote == -1)
            {
                comment.dislikes--;
                await networkClient.SendVote(comment.id, "undislike");
                votedMessages.Remove(comment.id);
            }
            else
            {
                if (currentVote == 1)
                {
                    comment.likes--;
                    await networkClient.SendVote(comment.id, "unlike");
                }

                comment.dislikes++;
                await networkClient.SendVote(comment.id, "dislike");
                votedMessages[comment.id] = -1;
            }

            itemUI.UpdateComment(comment);
            UpdateVoteColor(comment, itemUI);
        });


        itemUI.UpdateComment(comment);

    }
    private void UpdateVoteColor(CommentSchema comment, MessageItemUI itemUI)
    {
        Image upImg = itemUI.upVoteButton.GetComponent<Image>();
        Image downImg = itemUI.downVoteButton.GetComponent<Image>();
        Image frame = itemUI.voteImage.GetComponent<Image>();

        upImg.color = Color.white;
        downImg.color = Color.white;


        if (votedMessages.TryGetValue(comment.id, out int vote))
        {
            if (vote == 1)
            {
                upImg.color = Color.blue;            
                    frame.color = Color.blue;
                    itemUI.CountlikesText.color = Color.white;

            }
            else if (vote == -1)
            {
                downImg.color = Color.red;

                    frame.color = Color.red;
                    itemUI.CountlikesText.color = Color.white;
                
            }
        }
    }

    private void SendComment()
    {
        Debug.Log("SendComment được gọi");
        string commentText = inputField.text.Trim();
        if (string.IsNullOrEmpty(commentText))
        {
            Debug.LogWarning("Không thể gửi bình luận trống.");
            return;
        }

        networkClient.SendChatMessage(commentText);
     
        HideFormComment();
        CommentSchema newComment = new CommentSchema
        {
            id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), // tạm dùng timestamp làm id
            relationType = "comment",
            relationId = 0,
            parentId = 0,
            detail = commentText,
            replies = 0,
            likes = 0,
            dislikes = 0,
            createdAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            account = new AccountSchema
            {
                id = networkClient.id,
                name = networkClient.name,
                thumbnail = networkClient.thumbnail,
            },
            parent = null
        };
        Debug.Log("Gửi nội dung: " + commentText + ", ID: " + newComment.id);
        inputField.text = string.Empty; 
        AddComment(newComment, networkClient.level);
    }

    public void UpdateComment(CommentSchema comment)
    {
        if (commentViews.TryGetValue(comment.id, out MessageItemUI itemUI))
        {
            itemUI.UpdateComment(comment);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy comment ID {comment.id} để cập nhật.");
        }
    }

    public void ShowFormComment()
    {
        FormComment.SetActive(true);
        inputField.ActivateInputField();
    }
    public void HideFormComment()
    {
        FormComment.SetActive(false);
    }
}