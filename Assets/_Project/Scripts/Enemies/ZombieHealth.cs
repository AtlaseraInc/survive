using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public static event System.Action<ZombieHealth> OnZombieDied;

    public int maxHealth = 50;
    public float deathDelay = 1f; // match your death clip length

    private int currentHealth;
    private bool isDead;
    private Animator animator;

    public bool IsDead => isDead;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
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
        isDead = true;
        OnZombieDied?.Invoke(this);
        AudioManager.Instance?.PlayRandomSFX(AudioManager.Instance.zombieDeath);

        if (animator) animator.SetTrigger("Death");

        // Stop physics and collisions so the corpse doesn't block or hurt anyone
        var rb = GetComponent<Rigidbody2D>();
        if (rb) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, deathDelay);
    }
}