using System.Collections.Generic;

public static class RoomSession
{
    public static string RoomCode { get; private set; }
    public static string HostId { get; private set; }
    public static List<RoomInfo> RoomInfos { get; private set; } = new List<RoomInfo>();

    public static void Set(string code, string  hostId)
    {
        RoomCode = code;
        HostId = hostId;
    }

    public static void AddUser(string playerId, string nickName)
    {
        RoomInfos.Add(new RoomInfo { playerId = playerId, nickName = nickName });
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