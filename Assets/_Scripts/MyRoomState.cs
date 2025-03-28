using Colyseus.Schema;

public class MyRoomState : Schema
{
    [Type(0, "string")]
    public string message = "";
}
