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
    public static int UserId { get; private set; }
    public static string Nickname { get; private set; }

    public static void Set(int userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}