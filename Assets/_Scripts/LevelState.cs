using System.Collections.Generic;
using Colyseus.Schema;

public class Message : Schema
{
    public string id = "";

    public string sender = "";

    public string content = "";

    public long timestamp = 0;

    public string parentId = "";

    public int votes = 0;


    public List<Message> replies = new List<Message>();
}

public class LevelState : Schema
{
    public string levelName = "";

    public Dictionary<string, Message> messages = new();

}