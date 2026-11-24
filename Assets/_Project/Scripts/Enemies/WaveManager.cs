using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public int zombieCount = 5;
        public float spawnInterval = 1.5f;
        public float zombieSpeedMultiplier = 1f;
    }

    public static WaveManager Instance { get; private set; }

    [Header("References")]
    public GameObject zombiePrefab;

    [Header("Waves")]
    public Wave[] waves;
    public float timeBetweenWaves = 6f;
    public bool loopLastWave = true;   // keep escalating after the last defined wave
    public float loopScaling = 1.25f;  // zombie count multiplier per extra wave

    [Header("Spawning")]
    public float spawnOffsetX = 3f;    // distance past the right edge of the camera
    public float spawnY = -2f;         // ground height
    public int maxAlive = 15;

    public int CurrentWave { get; private set; }
    public int ZombiesAlive { get; private set; }

    public event System.Action<int> OnWaveStarted;
    public event System.Action<int> OnWaveCleared;

    private Camera cam;
    private int spawnedThisWave;
    private int waveTarget;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()  => ZombieHealth.OnZombieDied += HandleZombieDied;
    void OnDisable() => ZombieHealth.OnZombieDied -= HandleZombieDied;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        yield return new WaitForSeconds(3f); // grace period at the start

        while (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            CurrentWave++;
            Wave wave = GetWave(CurrentWave, out int count);
            if (wave == null) yield break; // no more waves and not looping

            waveTarget = count;
            spawnedThisWave = 0;

            OnWaveStarted?.Invoke(CurrentWave);
            HUDManager.Instance?.UpdateWave(CurrentWave);
            HUDManager.Instance?.ShowNotification($"Wave {CurrentWave}");
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.waveStart);

            // Spawn the wave
            while (spawnedThisWave < waveTarget)
            {
                if (ZombiesAlive < maxAlive)
                    SpawnZombie(wave.zombieSpeedMultiplier);

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            // Wait until every zombie in the wave is dead
            while (ZombiesAlive > 0)
                yield return null;

            OnWaveCleared?.Invoke(CurrentWave);
            HUDManager.Instance?.ShowNotification($"Wave {CurrentWave} cleared!");

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    Wave GetWave(int waveNumber, out int count)
    {
        count = 0;
        if (waves == null || waves.Length == 0) return null;

        if (waveNumber <= waves.Length)
        {
            Wave w = waves[waveNumber - 1];
            count = w.zombieCount;
            return w;
        }

        if (!loopLastWave) return null;

        // Past the last defined wave: scale up the final one
        Wave last = waves[waves.Length - 1];
        int extra = waveNumber - waves.Length;
        count = Mathf.RoundToInt(last.zombieCount * Mathf.Pow(loopScaling, extra));
        return last;
    }

    void SpawnZombie(float speedMultiplier)
    {
        float rightEdge = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        Vector3 pos = new Vector3(rightEdge + spawnOffsetX, spawnY, 0f);

        GameObject zombie = Instantiate(zombiePrefab, pos, Quaternion.identity);

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
            controller.moveSpeed *= speedMultiplier;

        spawnedThisWave++;
        ZombiesAlive++;
    }

    void HandleZombieDied(ZombieHealth zombie)
    {
        ZombiesAlive = Mathf.Max(0, ZombiesAlive - 1);
    }
}