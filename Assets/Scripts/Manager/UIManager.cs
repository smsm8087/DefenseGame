using UnityEngine;
using System.Collections.Generic;
using DataModels;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject cardSelectPopupPrefab;
    private CardSelectPopup cardSelectPopup;

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

    #region cardSelectPopup
    public void ShowCardSelectPopup(List<CardData> cards, float duration)
    {
        cardSelectPopup = PopupManager.Instance.ShowPopup<CardSelectPopup>(cardSelectPopupPrefab);
        cardSelectPopup.Init(cards, duration);
    }

    public void UpdateSettlementTimer(float duration)
    {
        cardSelectPopup?.UpdateTimer(duration);
    }
    public void UpdateSettlementReadyCount(int readyCount)
    {
        cardSelectPopup?.setCheckSlot(readyCount);
    }
    #endregion
    
}