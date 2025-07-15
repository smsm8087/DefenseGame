using UnityEngine;

public enum ServerMode
{
    Local,
    TestLocal,
    MultiPlay
}

[CreateAssetMenu(menuName = "Network/Server Config")]
public class ServerConfig : ScriptableObject
{
    public ServerMode serverMode = ServerMode.Local;
    public string TestLocalIp = "59.12.167.192";
    public string renderServerIp = "wss://defensegamewebsocketserver.onrender.com/ws";        

    public int port = 5215;

    public string GetServerIP()
    {
        switch (serverMode)
        {
            case ServerMode.Local:
                return $"ws://127.0.0.1:{port}/ws";
            case ServerMode.TestLocal:
                return $"ws://{TestLocalIp}:{port}/ws";
            case ServerMode.MultiPlay:
                return renderServerIp;
        }
        return $"ws://{TestLocalIp}:{port}/ws";
    }
}