using UnityEngine;

public class GemHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    
    [Header("HP Bar")]
    public HPBar hpBar;
    
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        
        // HP 바를 자동으로 찾기
        if (hpBar == null)
        {
            hpBar = GetComponentInChildren<HPBar>();
        }
        
        UpdateHPBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHPBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHPBar();
    }
    
    private void UpdateHPBar()
    {
        if (hpBar != null)
        {
            hpBar.UpdateHP(currentHealth, maxHealth);
        }
    }
    
    private void Die()
    {
        GameManager.Instance.GameOver();
    }
}