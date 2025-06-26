using UnityEngine;
using System.Collections.Generic;
using DataModels;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject cardSelectPopupPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowCardSelectPopup(List<CardData> cards, int duration)
    {
        var popup = PopupManager.Instance.ShowPopup<CardSelectPopup>(cardSelectPopupPrefab);
        popup.Init(cards, duration, OnCardSelected);
    }

    private void OnCardSelected(int selectedId)
    {
        Debug.Log($"[UIManager] 선택된 카드 ID: {selectedId}");
        // 필요하면 서버에 선택 결과 전송
    }
}