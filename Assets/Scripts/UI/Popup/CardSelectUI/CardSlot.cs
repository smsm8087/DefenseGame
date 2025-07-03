using System.Collections;
using System.Collections.Generic;
using DataModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private Image highlight;
    [SerializeField] private Image Icon;
    [SerializeField] private Image border;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private CardData cardData;
    private System.Action<int> onClick;
    private string baseResourcePath = "UI/skillcards_ui/";

    private bool isDuringAction = true;
    private bool isUILocked = false;
    
    public void Init(CardData cardData,  System.Action<int> onClickCallback)
    {
        this.cardData = cardData;
        
        setCard();
        setIconType();
        
        onClick = onClickCallback;
        SetSelected(false);
        canvasGroup.alpha = 0f;
        isUILocked = false;
    }

    public void StartCoroutine(float waitTime)
    {
        StartCoroutine(StartAnimation(waitTime));
    }
    private IEnumerator StartAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);  
        float duration = 0.3f;
        float time = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 from = startPos + new Vector3(-100f, 0f, 0f); // 왼쪽 오프셋
        Vector3 to = startPos;

        transform.localPosition = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            float eased = Utils.EaseOutCubic(t); 
            transform.localPosition = Vector3.Lerp(from, to, eased);
            
            canvasGroup.alpha = eased;

            yield return null;
        }

        transform.localPosition = to;
        canvasGroup.alpha = 1f;
        isDuringAction = false;
    }

    public void OnClick()
    {
        if (isDuringAction || isUILocked) return;
        onClick?.Invoke(cardData.id);
    }
    
    public void SetUILock(bool locked)
    {
        isUILocked = locked;
    }

    public void SetSelected(bool selected)
    {
        if (highlight != null)
            highlight.enabled = selected;
    }

    //TODO : 아이콘 작업 끝나면 작업 예정
    public void setIconType()
    {
        string type = cardData.type;
        switch (type)
        {
            case "add_criticalpct":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_criticalpercentageup");
            break;
            case "add_attack":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_damageup");
            break;
            case "add_movespeed":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_movspeedup");
            break;
            case "add_ultgauge":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_ultgaugeup");
            break;
            case "add_criticaldmg":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_criticaldamageup");
                break;
            case "add_hp":
                Icon.sprite = Resources.Load<Sprite>(baseResourcePath + "icon_healthicon");
            break;
        }
    }
    public void setCard()
    {
        string title = TextManager.Instance.GetText(cardData.title);
        string grade = cardData.grade;
        int value = cardData.value;
        string valueText = cardData.need_percent == 1 ? $"{value}%" : value.ToString();
        
        string hex = "";
        switch (grade)
        {
            case "normal":
                hex = "#75757b";
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "NORMAL_SKILLCARD");
                break;
            case "rare":
                hex = "#f2449c";
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "EPIC_SKILLCARD");
                break;
            case "legend":
                hex = "#ffc127";
                border.sprite = Resources.Load<Sprite>(baseResourcePath + "LEGENDARY_SKILLCARD");
                break; 
        }
        if (ColorUtility.TryParseHtmlString(hex, out Color gradeColor))
        {
            cardNameText.text = $"{title}\n<color={hex}><size=50>{valueText}</color>";
        }
    }
}