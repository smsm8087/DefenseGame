using System;
using System.Collections.Generic;
using DataModels;
using Newtonsoft.Json;
using UnityEngine;


public class StartGameHandler : INetworkMessageHandler
{
    private readonly Action callback;
    public string Type => "started_game";

    public StartGameHandler(Action callback)
    {
        this.callback = callback;
    }
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("IngameScene", () =>
        {
            //IngameScene이 로드 되었으므로
            var message = new
            {
                type = "scene_loaded",
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode,
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
            callback?.Invoke();
        });
    }
}
public class CreateRoomHandler : INetworkMessageHandler
{
    public string Type => "room_created";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"방 생성 성공! 코드: {RoomSession.RoomCode}");
        }); 
    }
}
public class JoinRoomHandler : INetworkMessageHandler
{
    public string Type => "room_joined";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"입장 성공! 코드: {RoomSession.RoomCode}");
        }); 
    }
}
public class RoomInfoHandler : INetworkMessageHandler
{
    public string Type => "room_info";
    private readonly Action callback; 
    public RoomInfoHandler(Action callback)
    {
        this.callback = callback;
    }

    public void Handle(NetMsg msg)
    {
        RoomSession.Set(msg.roomCode, msg.hostId);
        List<RoomInfo> RoomInfos = msg.RoomInfos;
        foreach (var roomInfo in RoomInfos)
        {
            if (RoomSession.RoomInfos.Find(x=> x.playerId == roomInfo.playerId) != null) continue;
            RoomSession.AddUser(roomInfo.playerId, roomInfo.nickName);
        }

        if (RoomInfos.Count != RoomSession.RoomInfos.Count)
        {
            //추가를 했는데도 맞지않으면 룸인포에서 삭제해야함.
            for (int i = 0; i < RoomSession.RoomInfos.Count; i++)
            {
                RoomInfo roomInfo = RoomSession.RoomInfos[i];
                if (RoomInfos.Find(x => x.playerId == roomInfo.playerId) != null) continue;
                //방을 나간 유저 룸인포에서 삭제
                RoomSession.RemoveUser(roomInfo.playerId);
            }
        }
        callback?.Invoke();
    }
}
public class OutRoomHandler : INetworkMessageHandler
{
    public string Type => "out_room";
    public void Handle(NetMsg msg)
    {
        var roomInfo = RoomSession.RoomInfos.Find(x => x.playerId == msg.playerId);
        if (roomInfo != null)
        {
            Debug.Log($"이전에 룸인포에서 삭제가 안된모양임. 확인 필요");
            RoomSession.RoomInfos.Remove(roomInfo);
        }
        SceneLoader.Instance.LoadScene("LobbyScene", () =>
        {
            Debug.Log($"방 나가기 성공!");
        }); 
    }
}
public class ChatRoomHandler : INetworkMessageHandler
{
    public string Type => "chat_room";
    
    private ChatUIManager chatUI;

    public class ChatMessage
    {
        public string playerId;
        public string nickName;
        public string message;
        public DateTime timestamp = DateTime.UtcNow;
    }
    
    private static Dictionary<string, List<ChatMessage>> chatLog = new Dictionary<string, List<ChatMessage>>();

    public static void Save(ChatMessage msg)
    {
        if (!chatLog.ContainsKey(RoomSession.RoomCode))
        {
            chatLog[RoomSession.RoomCode] = new List<ChatMessage>();
        }

        chatLog[RoomSession.RoomCode].Add(msg);

        Debug.Log($"[채팅 저장됨] {msg.nickName} ({msg.playerId}): {msg.message} @ {msg.timestamp:HH:mm:ss}");
    }

    public static void PrintAllLogs()
    {
        foreach (var pair in chatLog)
        {
            Debug.Log($"▶ {pair.Key}방의 채팅 기록:");
            foreach (var msg in pair.Value)
            {
                Debug.Log($"   [{msg.timestamp:HH:mm:ss}] {msg.nickName} ({msg.playerId}) : {msg.message}");
            }
        }
    }
    
    public ChatRoomHandler()
    {
        // ChatUIManager 싱글톤 또는 GameObject 찾기 방식
        chatUI = GameObject.FindObjectOfType<ChatUIManager>();
        if (chatUI == null)
            Debug.LogError("ChatUIManager를 찾을 수 없습니다!");
    }
    
    public void Handle(NetMsg msg)
    {
        Debug.Log($"채팅창 전송 성공! 코드: {RoomSession.RoomCode}");
        
        // ✅ 채팅 저장
        var chatMessage = new ChatMessage
        {
            playerId = msg.playerId,
            nickName = msg.nickName,
            message = msg.message
        };

        ChatRoomHandler.Save(chatMessage);
        ChatRoomHandler.PrintAllLogs();
        
        // UI에 메시지 추가
        if (chatUI != null)
        {
            chatUI.AddChatMessage(msg.nickName, msg.message);
        }
        CharacterSelectSceneManager.Instance.setUiLock(false);
    }
}
public class SelectedCharacterHandler : INetworkMessageHandler
{
    public string Type => "selected_character";
    
    public void Handle(NetMsg msg)
    {
        CharacterSelectSceneManager.Instance.UpdatePlayerIcon(msg.playerId, msg.jobType);
        //캐릭터를 선택하면 준비가 된 것임
        CharacterSelectSceneManager.Instance.SetReady(msg.playerId, true);
        CharacterSelectSceneManager.Instance.setUiLock(false);
        //다 준비되었을때 토스트메시지 띄워줌.
        if (msg.isAllReady)
        {
            CharacterSelectSceneManager.Instance.MoveReadyText();
        }
    }
}
public class DeSelectedCharacterHandler : INetworkMessageHandler
{
    public string Type => "deselected_character";
    
    public void Handle(NetMsg msg)
    {
        CharacterSelectSceneManager.Instance.SetReady(msg.playerId, false);
        CharacterSelectSceneManager.Instance.StopMoveReadyTextCoroutine();
        CharacterSelectSceneManager.Instance.setUiLock(false);
    }
}
