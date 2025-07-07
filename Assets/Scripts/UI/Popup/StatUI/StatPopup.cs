using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DataModels;
using TMPro;

public class StatPopup : BasePopup
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI nickText;
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private Transform cardPanelTransform;
    [SerializeField] private Transform statLeftTransform;
    [SerializeField] private Transform statRightTransform;

    [SerializeField] private GameObject miniCardPrefab;
    [SerializeField] private GameObject statPrefab;
    
    [SerializeField] private UICharacterAnimator characterAnimator;
    [SerializeField] private GameObject emptyTextObj;
    

    private PlayerInfo playerinfo;
    public void Init(PlayerInfo playerinfo)
    {
        this.playerinfo = playerinfo;
        setJobText();
        setCards();
        setStats();
        closeBtn?.onClick.AddListener(OnClose);
    }

    private void setJobText()
    {
        jobText.text = TextManager.Instance.GetText(playerinfo.job_type);
        characterAnimator.SetJob(playerinfo.job_type);
    }

    private void setCards()
    {
        foreach (Transform child in cardPanelTransform)
            Destroy(child.gameObject);
        
        var cardTable = GameDataManager.Instance.GetTable<CardData>("card_data");
        for (int i = 0; i < playerinfo.cardIds.Count; i++)
        {
            int cardId =  playerinfo.cardIds[i];
            CardData cardData = cardTable[cardId];
            var miniCardObj = Instantiate(miniCardPrefab, cardPanelTransform);
            var miniCardSlot = miniCardObj.GetComponent<MiniCardSlot>();
            miniCardSlot.Init(cardData);
        }
        emptyTextObj.SetActive(playerinfo.cardIds.Count == 0);
    }

    private void setStats()
    {
        //left
        var attack_statObj = Instantiate(statPrefab, statLeftTransform);
        var attack_statSlot = attack_statObj.GetComponent<StatSlot>();
        attack_statSlot.Init(TextManager.Instance.GetText("attack_power"), playerinfo.currentAttack.ToString());
        
        var attackSpeed_statObj = Instantiate(statPrefab, statLeftTransform);
        var attackSpeed_statSlot = attackSpeed_statObj.GetComponent<StatSlot>();
        attackSpeed_statSlot.Init(TextManager.Instance.GetText("attack_speed"), playerinfo.currentAttackSpeed.ToString("F2"));
        
        var hp_statObj = Instantiate(statPrefab, statLeftTransform);
        var hp_statSlot = hp_statObj.GetComponent<StatSlot>();
        hp_statSlot.Init(TextManager.Instance.GetText("hp"), playerinfo.currentMaxHp.ToString());
        
        var moveSpeed_statObj = Instantiate(statPrefab, statLeftTransform);
        var moveSpeed_statSlot = moveSpeed_statObj.GetComponent<StatSlot>();
        moveSpeed_statSlot.Init(TextManager.Instance.GetText("move_speed"), playerinfo.currentMoveSpeed.ToString());
        
        //right
        var ultgauge_statObj = Instantiate(statPrefab, statRightTransform);
        var ultgauge_statSlot = ultgauge_statObj.GetComponent<StatSlot>();
        ultgauge_statSlot.Init(TextManager.Instance.GetText("ultgauge"), playerinfo.currentUltGauge.ToString());
        
        var cri_pct_statObj = Instantiate(statPrefab, statRightTransform);
        var cri_pct_statSlot = cri_pct_statObj.GetComponent<StatSlot>();
        cri_pct_statSlot.Init(TextManager.Instance.GetText("cri_pct"), playerinfo.currentCriPct.ToString());
        
        var cri_dmg_statObj = Instantiate(statPrefab, statRightTransform);
        var cri_dmg_statSlot = cri_dmg_statObj.GetComponent<StatSlot>();
        cri_dmg_statSlot.Init(TextManager.Instance.GetText("cri_dmg"), playerinfo.currentCriDmg.ToString());
    }
    private void OnClose()
    {
        if (ui_lock) return;
        ui_lock = true;
        Close();
    }
}

