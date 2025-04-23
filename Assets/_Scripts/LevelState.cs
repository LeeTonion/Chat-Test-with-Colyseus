using Colyseus.Schema;

public class AccountSchema : Schema
{

    public long id = 0;


    public string name = "";


    public string thumbnail = "";
}

public class ParentCommentSchema : Schema
{

    public long id = 0;

    public string detail = "";


    public AccountSchema account = new AccountSchema();
}

public class CommentSchema : Schema
{

    public long id = 0;


    public long accountId = 0;

    public string relationType = "";


    public long relationId = 0;


    public long parentId = 0;


    public string detail = "";

    public int replies = 0;


    public int likes = 0;

    public int dislikes = 0;

    public long createdAt = 0;


    public long updatedAt = 0;


    public AccountSchema account = new AccountSchema();


    public ParentCommentSchema parent = new ParentCommentSchema();
}

public class MyRoomState : Schema
{

    public ArraySchema<CommentSchema> comments = new ArraySchema<CommentSchema>();
}