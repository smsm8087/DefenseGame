using System;
using Unity.VisualScripting;
using UnityEngine;

public class GemHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    
    [Header("HP Bar")]
    public HPBar hpBar;
    public HPText hpText;
    
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        UpdateHPBar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(5);
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
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

        if (hpText != null)
        {
            hpText.UpdateHP(currentHealth, maxHealth);
        }
    }
    
    private void Die()
    {
        GameManager.Instance.GameOver();
    }
}