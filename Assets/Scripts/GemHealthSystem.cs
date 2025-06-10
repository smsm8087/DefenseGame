using System;
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
    }

    private void Update()
    {
        UpdateHPBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
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