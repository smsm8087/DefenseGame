using UnityEngine;
using System.Collections.Generic;
using DataModels;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject cardSelectPopupPrefab;
    [SerializeField] private GameObject statUIPrefab;
    private CardSelectPopup cardSelectPopup;
    private StatPopup statUIPopup;

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
    #region statPopup
    public void ShowStatPopup()
    {
        statUIPopup = PopupManager.Instance.ShowPopup<StatPopup>(statUIPrefab);
        statUIPopup.Init();
    }
    #endregion
    
}