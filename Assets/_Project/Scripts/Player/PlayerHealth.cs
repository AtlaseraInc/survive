using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private bool isDead;
    public bool IsDead => isDead;

    public event System.Action<int, int> OnHealthChanged; // current, max

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        // AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.playerHurt); // enable after merging feature/audio
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        var animator = GetComponent<Animator>();
        if (animator) animator.SetTrigger("Death");

        GameManager.Instance.TriggerGameOver();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}