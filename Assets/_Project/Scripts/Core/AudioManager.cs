using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music (by scene)")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;

    [Header("Player SFX")]
    public AudioClip[] meleeSwing;
    public AudioClip[] shoot;
    public AudioClip[] playerHurt;
    public AudioClip jump;

    [Header("Zombie SFX")]
    public AudioClip[] zombieGroan;
    public AudioClip[] zombieAttack;
    public AudioClip[] zombieDeath;

    [Header("World SFX")]
    public AudioClip friendRescued;
    public AudioClip waveStart;
    public AudioClip buttonClick;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumes();
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":  PlayMusic(menuMusic); break;
            case "GameScene": PlayMusic(gameMusic); break;
            case "GameOver":  PlayMusic(gameOverMusic, loop: false); break;
            // LoadingScreen: keep whatever is playing
        }
    }

    // ── Music ───────────────────────────────────────────

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource) musicSource.Stop();
    }

    // ── SFX ─────────────────────────────────────────────

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // Picks a random clip and varies the pitch slightly so repeats don't sound identical
    public void PlayRandomSFX(AudioClip[] clips, float volumeScale = 1f, float pitchVariation = 0.1f)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        sfxSource.PlayOneShot(clip, volumeScale);
        sfxSource.pitch = 1f;
    }

    // ── Volume ──────────────────────────────────────────

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        ApplyVolumes();
    }

    void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource)   sfxSource.volume = sfxVolume;
    }
}