using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataModels;

public class CardSelectPopup : BasePopup
{
    [SerializeField] private List<CardSlot> cardSlots;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image timeBarImage;

    private int selectedCardId = 0;
    private int remainTime = 0;
    private int duration = 0;
    private Dictionary<int, CardSlot> cardSlotMap = new();

    private System.Action<int> onConfirmCallback;

    public void Init(List<CardData> cardList, int duration, System.Action<int> onConfirm)
    {
        onConfirmCallback = onConfirm;
        this.duration = remainTime = duration;
        for (int i = 0; i < cardList.Count; i++)
        {
            var slot = cardSlots[i];
            slot.Init(cardList[i], OnCardClicked);
            slot.StartCoroutine((i + 1) * 0.1f);
            cardSlotMap[cardList[i].id] = slot;
        }

        confirmButton?.onClick.AddListener(OnConfirmClicked);
        cancelButton?.onClick.AddListener(OnCancelClicked);
        confirmButton.interactable = false;
        timeBarImage.fillAmount = Mathf.Clamp01(remainTime / duration);
    }

    private void OnCardClicked(int cardId)
    {
        selectedCardId = cardId;

        // 모든 슬롯 비선택 상태로
        foreach (var slot in cardSlotMap.Values)
            slot.SetSelected(false);

        // 선택한 슬롯만 강조
        cardSlotMap[cardId].SetSelected(true);
        confirmButton.interactable = true;
    }

    private void OnConfirmClicked()
    {
        onConfirmCallback?.Invoke(selectedCardId);
        Close();
    }

    private void OnCancelClicked()
    {
        Close();
    }
}