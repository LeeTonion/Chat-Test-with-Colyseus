using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public class JWTGenerator : MonoBehaviour
{
    [Header("User Info")]
    [SerializeField] private long id;
    [SerializeField] private string name;
    [SerializeField] private string thumbnail;

    [Header("Secret Key")]
    [SerializeField] private string secretKey = "mySecretKey";

    [ContextMenu("Generate JWT Token")]
    public void GenerateToken()
    {
        string token = CreateJWT(id, name, thumbnail, secretKey);
        Debug.Log("JWT Token:\n" + token);
    }

    public string CreateJWT(long id, string name, string thumbnail, string secretKey)
    {
        // Header
        string header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
        string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(header));

        // Payload
        string payload = "{" +
            "\"id\":" + id + "," +
            "\"name\":\"" + EscapeJson(name) + "\"," +
            "\"thumbnail\":\"" + EscapeJson(thumbnail) + "\"" +
        "}";
        string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payload));

        // Signature
        string message = encodedHeader + "." + encodedPayload;
        string signature = CreateHMACSHA256Signature(message, secretKey);

        return message + "." + signature;
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private string CreateHMACSHA256Signature(string message, string secret)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        using (var hasher = new HMACSHA256(keyBytes))
        {
            byte[] hash = hasher.ComputeHash(messageBytes);
            return Base64UrlEncode(hash);
        }
    }

    private string EscapeJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
