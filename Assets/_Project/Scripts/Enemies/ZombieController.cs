using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float moveSpeed = 1.8f;
    public float detectionRange = 6f;
    public float attackRange = 0.6f;
    public int attackDamage = 10;
    public float attackCooldown = 1.2f;

    private Transform player;
    private float attackTimer = 0f;
    private Animator animator;
    private ZombieHealth health;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        health = GetComponent<ZombieHealth>();
    }

    void Update()
    {
        if (health != null && health.IsDead) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        if (dist <= attackRange)
            Attack();
        else if (dist <= detectionRange)
            Chase();
        else
            Idle();
    }

    void Chase()
    {
        if (animator) animator.SetBool("IsWalking", true);

        // Horizontal only, so zombies stay on the ground
        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        transform.Translate(Vector2.right * dirX * moveSpeed * Time.deltaTime);

        // Flip to face the player
        transform.localScale = new Vector3(
            dirX < 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z);
    }

    void Attack()
    {
        if (animator) animator.SetBool("IsWalking", false);

        if (attackTimer <= 0f)
        {
            if (animator) animator.SetTrigger("Attack");
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
        }
    }

    void Idle()
    {
        if (animator) animator.SetBool("IsWalking", false);
    }
}