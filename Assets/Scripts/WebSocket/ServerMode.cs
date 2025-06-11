using UnityEngine;

public enum ServerMode
{
    Local,
    Multiplayer
}

[CreateAssetMenu(menuName = "Network/Server Config")]
public class ServerConfig : ScriptableObject
{
    public ServerMode serverMode = ServerMode.Local;
    public string multiplayerIP = "59.12.167.192";
    public int port = 5215;

    public string GetServerIP()
    {
        return serverMode == ServerMode.Local ? "127.0.0.1" : multiplayerIP;
    }
}