using System.Collections.Generic;

[System.Serializable]
public class NetMsg
{
    public string type;
    public string playerId;
    public List<string> players;
    public float x;
    public float y;
}