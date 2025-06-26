using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataModels;
using TMPro;

public class CardSelectPopup : BasePopup
{
    [SerializeField] private List<CardSlot> cardSlots;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image timeBarImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI btnText;

    private int selectedCardId = 0;
    private float duration = 0;
    private Dictionary<int, CardSlot> cardSlotMap = new();

    private float currentFill = 1f;          
    private float targetFill = 1f;           
    private float lerpSpeed = 5f;   

    public void Init(List<CardData> cardList, float duration)
    {
        this.duration = duration;
        for (int i = 0; i < cardList.Count; i++)
        {
            var slot = cardSlots[i];
            slot.Init(cardList[i], OnCardClicked);
            slot.StartCoroutine((i + 1) * 0.1f);
            cardSlotMap[cardList[i].id] = slot;
        }

        confirmButton?.onClick.AddListener(OnConfirmClicked);
        confirmButton.interactable = false;
        timeBarImage.fillAmount = Mathf.Clamp01(duration / duration);
        titleText.text = TextManager.Instance.GetText("popup_title_select_card");
        btnText.text = TextManager.Instance.GetText("btn_select");
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
        var msg = new NetMsg
        {
            type = "settlement_ready",
            playerId = NetworkManager.Instance.MyGUID,
            selectedCardId = selectedCardId
        };

        NetworkManager.Instance.SendMsg(msg);
        Close();
    }
    public void UpdateTimer(float remainTime)
    {
        targetFill = Mathf.Clamp01(remainTime / duration);
    }

    private void Update()
    {
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * lerpSpeed);
        timeBarImage.fillAmount = currentFill;
    }
}