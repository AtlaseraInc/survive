using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    private bool isGrounded;
    private bool facingRight = true;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerHealth health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Stop horizontal movement once dead, but let gravity keep working
        if (health != null && health.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator) animator.SetFloat("Speed", 0f);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");

        HandleMovement(h);
        HandleJump();
        HandleFlip(h);
    }

    // Handle horizontal movement and update animator
    void HandleMovement()
    {
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        if (animator) animator.SetFloat("Speed", Mathf.Abs(h));
    }

    // Handle jumping and ground check
    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator) animator.SetTrigger("Jump");
            // AudioManager.Instance?.PlaySFX(AudioManager.Instance.jump); // enable after merging feature/audio
        }

        if (animator) animator.SetBool("IsGrounded", isGrounded);
    }

    // Flip the player sprite based on movement direction
    void HandleFlip()
    {
        if ((h > 0 && !facingRight) || (h < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }
}