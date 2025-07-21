public static class RoomSession
{
    public static string RoomCode { get; private set; }

    public static void Set(string code)
    {
        RoomCode = code;
    }
}
public class UserSession
{
    public static string UserId { get; private set; }
    public static string Nickname { get; private set; }

    public static void Set(string userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}