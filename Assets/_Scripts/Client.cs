using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Colyseus;
using System;
using System.Linq;
using Colyseus.Schema;

public class Client : MonoBehaviour
{
    [SerializeField] private string serverUrl = "ws://192.168.1.131:2567";
    [SerializeField] private string roomID = "";
    [SerializeField] public long id;
    [SerializeField] public string name;
    [SerializeField] public string thumbnail;
    [SerializeField] private string levelId = "278";
    [SerializeField] public string level = "1";

    [SerializeField] private JWTGenerator JWTGenerator;
    [SerializeField] private ChatUIManager chatUIManager;
    private ColyseusClient client;
    private ColyseusRoom<MyRoomState> room;
    private string jwtToken;

    private async void Start()
    {
        jwtToken = JWTGenerator.CreateJWT(id, name, thumbnail, "mTn8MCSFLSAe1DwyTPrnIR7gk7tslm71");
        await InitializeClient();
        await JoinRoom();
    }

    private async Task InitializeClient()
    {
        try
        {
            if (string.IsNullOrEmpty(serverUrl) || !serverUrl.StartsWith("ws://"))
                throw new Exception("Server URL không hợp lệ.");

            client = new ColyseusClient(serverUrl);
            Debug.Log("✅ Khởi tạo ColyseusClient thành công.");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Lỗi khởi tạo ColyseusClient: " + ex.Message);
        }
    }

    public async Task<bool> JoinRoom()
    {
        if (client == null)
        {
            Debug.LogError("⚠️ Client chưa được khởi tạo.");
            return false;
        }

        if (string.IsNullOrEmpty(roomID) || string.IsNullOrEmpty(jwtToken))
        {
            Debug.LogError("⚠️ roomID hoặc jwtToken không hợp lệ.");
            return false;
        }

        try
        {
            var options = new Dictionary<string, object>
            {
                { "level", level },
                { "levelId", levelId }
            };
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + jwtToken }
            };

            room = await client.JoinById<MyRoomState>(roomID, options, headers);
            if (room == null)
            {
                Debug.LogError("❌ Phòng trả về null sau khi tham gia.");
                return false;
            }

            Debug.Log("✅ Đã tham gia phòng thành công: " + roomID);

            room.Send("getCommentsData");
            room.OnMessage<CommentSchema[]>("commentsData", (comments) =>
            {
                Debug.Log("Nhận comments từ server:");

                foreach (var comment in comments)
                {
                    chatUIManager.AddComment(comment, level);
                }

            });

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Tham gia phòng thất bại (roomID: {roomID}): {ex.Message}");
            return false;
        }
    }


    public async Task SendChatMessage(string detail, long parentId = 0)
    {
        if (room == null)
        {
            Debug.LogWarning("Chưa kết nối tới phòng.");
            return;
        }

        if (string.IsNullOrEmpty(detail))
        {
            Debug.LogWarning("Nội dung bình luận rỗng.");
            return;
        }

        var message = new Dictionary<string, object>
        {
            { "type", "comment" },
            { "detail", detail }
        };

        if (parentId != 0)
        {
            message["parentId"] = parentId;
        }

        try
        {
            await room.Send("message", message);
            room.Send("getCommentsData");
            room.OnMessage<CommentSchema[]>("commentsData", (comments) =>
            {
                Debug.Log("Nhận comments từ server:");

                foreach (var comment in comments)
                {
                    chatUIManager.AddComment(comment, level);
                }

            });
            Debug.Log($"✅ Gửi bình luận: detail={detail}, parentId={parentId}, accountId={id}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi gửi bình luận: {ex.Message}");
        }
    }
    public async Task SendVote(long commentId, string voteType)
    {
        if ( room == null)
        {
            Debug.LogWarning("Chưa kết nối.");
            return;
        }

        if (commentId == 0 || !new[] { "like", "dislike", "unlike", "undislike" }.Contains(voteType))
        {
            Debug.LogWarning("Tham số vote không hợp lệ.");
            return;
        }

        var message = new Dictionary<string, object>
        {
            { "type", voteType },
            { "id", commentId }
        };

        try
        {
            await room.Send("message", message);
            Debug.Log($"✅ Gửi vote '{voteType}' cho bình luận: {commentId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi gửi vote: {ex.Message}");
        }
    }
    private void OnDestroy()
    {
        if (room != null)
        {
            room.Leave();
            room = null;
        }
    }
}