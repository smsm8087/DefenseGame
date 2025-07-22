public static class RoomSession
{
    public static string RoomCode { get; private set; }
    public static string HostId { get; private set; }

    public static void Set(string code, string  hostId)
    {
        RoomCode = code;
        HostId = hostId;
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