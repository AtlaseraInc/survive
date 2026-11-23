using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.zombieDeath);
        Destroy(gameObject);
    }
}