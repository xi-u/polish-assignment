using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0.35f;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        PlayMusic(Resources.Load<AudioClip>("Audio/space"), 0.1f);

        if (Camera.main != null && Camera.main.GetComponent<AudioListener>() == null)
        {
            Camera.main.gameObject.AddComponent<AudioListener>();
        }
    }

    public static void EnsureInstanceExists()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject audioManagerObject = new GameObject("AudioManager");
        audioManagerObject.AddComponent<AudioManager>();
    }

    public void Play(AudioClip clip, Vector3 position, float volume = 0.6f)
    {
        if (clip == null)
        {
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.spatialBlend = 0.35f;
        audioSource.volume = volume;
        audioSource.transform.position = position;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume = 0.35f)
    {
        if (clip == null)
        {
            return;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;
        }

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}