using UnityEngine;
using System.Collections.Generic;
using DataModels;
using TMPro;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager Instance { get; private set; }

    [Header("채팅 UI 연결")]
    public GameObject chatPrefab;                // Chatting 프리팹
    public Transform chatContentParent;          // ScrollView > Content
    // public ScrollRect scrollRect;                // ScrollView (자동 스크롤용)

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddChatMessage(string nickname, string message)
    {
        GameObject newChat = Instantiate(chatPrefab, chatContentParent);

        TMP_Text nicknameText = newChat.transform.Find("NickName").GetComponent<TMP_Text>();
        TMP_Text messageText = newChat.transform.Find("Text").GetComponent<TMP_Text>();

        nicknameText.text = nickname;
        messageText.text = message;

        // 스크롤 맨 아래로
        // Canvas.ForceUpdateCanvases();
        // scrollRect.verticalNormalizedPosition = 0f;
    }
}