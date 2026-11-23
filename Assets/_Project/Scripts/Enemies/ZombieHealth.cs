using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public static event System.Action<ZombieHealth> OnZombieDied;

    public int maxHealth = 50;
    private int currentHealth;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

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