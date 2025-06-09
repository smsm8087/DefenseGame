using UnityEngine;

public class HPBar : MonoBehaviour
{
    [Header("HP Bar Components")]
    public Transform fillTransform;
    public SpriteRenderer fillRenderer;
    
    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    
    private Vector3 originalFillScale;
    
    void Start()
    {
        if (fillTransform == null)
        {
            fillTransform = transform.Find("Fill");
        }
        
        if (fillRenderer == null && fillTransform != null)
        {
            fillRenderer = fillTransform.GetComponent<SpriteRenderer>();
        }
        
        if (fillTransform != null)
        {
            originalFillScale = fillTransform.localScale;
        }
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (fillTransform == null) return;
        
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        
        Vector3 newScale = originalFillScale;
        newScale.x = originalFillScale.x * healthPercent;
        fillTransform.localScale = newScale;
        
        if (fillRenderer != null)
        {
            fillRenderer.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
        
        gameObject.SetActive(currentHP > 0);
    }
}