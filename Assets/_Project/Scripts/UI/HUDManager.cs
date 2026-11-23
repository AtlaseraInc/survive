using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Health")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Friends")]
    public TextMeshProUGUI friendsText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Ammo")]
    public TextMeshProUGUI ammoText;

    [Header("Waves")]
    public TextMeshProUGUI waveText;

    [Header("Notifications")]
    public TextMeshProUGUI notificationText;
    private Coroutine notificationCoroutine;

    private PlayerHealth playerHealth;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Hook into player health events and show the starting value
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealth;
            UpdateHealth(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        // Hook into game manager events
        if (GameManager.Instance != null)
            GameManager.Instance.OnFriendRescued += UpdateFriends;

        // Initialize UI
        UpdateFriends(0);
        UpdateWave(0);
        HideNotification();
    }

    void Update()
    {
        UpdateTimer();
    }

    // ── Health ───────────────────────────────────────────

    void UpdateHealth(int current, int max)
    {
        if (healthBar)
            healthBar.value = (float)current / max;

        if (healthText)
            healthText.text = $"{current} / {max}";
    }

    // ── Friends ──────────────────────────────────────────

    void UpdateFriends(int count)
    {
        if (!friendsText) return;

        int total = GameManager.Instance != null ? GameManager.Instance.totalFriends : 4;
        friendsText.text = $"Friends: {count} / {total}";
    }

    // ── Timer ────────────────────────────────────────────

    void UpdateTimer()
    {
        if (timerText)
            timerText.text = FormatTime(SurvivalTimer.ElapsedTime);
    }

    string FormatTime(float seconds)
    {
        int mins = (int)(seconds / 60);
        int secs = (int)(seconds % 60);
        return $"{mins:00}:{secs:00}";
    }

    // ── Ammo ─────────────────────────────────────────────

    public void UpdateAmmo(int current, int max)
    {
        if (ammoText)
            ammoText.text = $"{current} / {max}";
    }

    // ── Waves ────────────────────────────────────────────

    public void UpdateWave(int wave)
    {
        if (waveText)
            waveText.text = wave > 0 ? $"Wave {wave}" : "";
    }

    // ── Notifications ────────────────────────────────────

    public void ShowNotification(string message, float duration = 2.5f)
    {
        if (!notificationText) return;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(DisplayNotification(message, duration));
    }

    IEnumerator DisplayNotification(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        HideNotification();
    }

    void HideNotification()
    {
        if (notificationText)
            notificationText.gameObject.SetActive(false);
    }

    // ── Cleanup ──────────────────────────────────────────

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;

        if (GameManager.Instance != null)
            GameManager.Instance.OnFriendRescued -= UpdateFriends;
    }
}