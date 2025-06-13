using System;
using UnityEngine;

public class GemHealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public HPBar hpBar;
    public HPText hpText;

    public Action OnDeath;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPBar();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(5);
            NetworkManager.Instance.RemoveEnemy(other.gameObject.GetComponent<EnemyMovement>().guid);
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHPBar();

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        hpBar?.UpdateHP(currentHealth, maxHealth);
        hpText?.UpdateHP(currentHealth, maxHealth);
    }
}