﻿using System;
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
    [SerializeField] private GameObject readyPrefab;
    [SerializeField] private Transform readySlotTransform;
    
    private List<ReadyIcon> readySlots = new List<ReadyIcon>();

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
        
        foreach (Transform child in readySlotTransform)
            Destroy(child.gameObject);
        int readySlotCount = NetworkManager.Instance.GetPlayers().Count;
        
        for(int i = 0; i < readySlotCount; i++)
        {
            var slot = Instantiate(readyPrefab, readySlotTransform);
            readySlots.Add(slot.GetComponent<ReadyIcon>());
        }
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
        if (ui_lock) return;
        ui_lock = true;
        confirmButton.interactable = false;
        
        foreach (var slot in cardSlotMap.Values)
        {
            slot.SetUILock(true);
        }
        
        var msg = new NetMsg
        {
            type = "settlement_ready",
            playerId = NetworkManager.Instance.MyGUID,
            selectedCardId = selectedCardId
        };

        NetworkManager.Instance.SendMsg(msg);
    }
    public void UpdateTimer(float remainTime)
    {
        targetFill = Mathf.Clamp01(remainTime / duration);
    }

    public void setCheckSlot(int readyCount)
    {
        if (readySlots.Count < readyCount) return;
        foreach (var readySlot in readySlots)
        {
            readySlot.SetSelected(false);
        }
        
        for (int i = 0; i < readyCount; i++)
        {
            readySlots[i].SetSelected(true);
        }

        if (readySlots.Count == readyCount)
        {
            Close();
        }
    }

    private void Update()
    {
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * lerpSpeed);
        timeBarImage.fillAmount = currentFill;
    }
}