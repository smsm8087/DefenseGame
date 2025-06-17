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
        hpImg.fillAmount = healthPercent;
        
        UpdateEmoji(currentHP);
    }
    
    void UpdateEmoji(float currentHP)
    {
        if (!emojiImage) return;
        
        // HP 값에 따라 이모지 변경 
        if (currentHP > 66f) 
        {
            emojiImage.sprite = hp100Emoji;
        }
        else if (currentHP > 33f)
        {
            emojiImage.sprite = hp50Emoji;
        }
        else 
        {
            emojiImage.sprite = hp0Emoji;
        }
        
        // 이모지 표시
        emojiImage.gameObject.SetActive(true);
    }
}