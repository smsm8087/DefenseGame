using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("HP Bar")]
    Image hpImg;
    
    [Header("HP Emoji")]
    public Image emojiImage; 
    public Sprite hp100Emoji; 
    public Sprite hp50Emoji;  
    public Sprite hp0Emoji;   
    
    void Awake()
    {
        hpImg = GetComponent<Image>();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpImg) return;
        
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        StartCoroutine(LerpHp(healthPercent));
        UpdateEmoji(healthPercent * 100f);
    }

    public IEnumerator LerpHp(float targetPercent)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startFill = hpImg.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hpImg.fillAmount = Mathf.Lerp(startFill, targetPercent, elapsed / duration);
            yield return null;
        }

        hpImg.fillAmount = targetPercent; // 마지막 값 정확히 맞춤
        // 이모지 표시
        emojiImage.gameObject.SetActive(targetPercent > 0);
    }
    
    void UpdateEmoji(float healthPercent)
    {
        if (!emojiImage) return;
        
        // HP 값에 따라 이모지 변경 
        if (healthPercent > 66f) 
        {
            emojiImage.sprite = hp100Emoji;
        }
        else if (healthPercent > 33f)
        {
            emojiImage.sprite = hp50Emoji;
        }
        else 
        {
            emojiImage.sprite = hp0Emoji;
        }
    }
}