using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private MusicLibrary musicLibrary;
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Mixer controls volume — AudioSource stays at 1
        musicSource.volume = 0.1f;
    }

    private void Update()
    {
        // ⚠ REMOVE linear mixing! Leave this empty or remove Update entirely.
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    public void StopMusic(float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(null, fadeDuration));
    }

    // ✔ YOUR ORIGINAL CROSSFADE REMAINS EXACTLY THE SAME
    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        float startVol = musicSource.volume;


        if (musicSource.clip == nextTrack)
        {
            yield break; // No need to change music
        }

        // Fade OUT the current clip
        while (percent < 1)
        {
            //percent += Time.deltaTime / fadeDuration;
            //musicSource.volume = Mathf.Lerp(startVol, 0, percent);
            //yield return null;
            percent += Time.unscaledDeltaTime / fadeDuration; // Gunakan unscaledDeltaTime
            musicSource.volume = Mathf.Lerp(startVol, 0, percent);
            yield return new WaitForSecondsRealtime(0f); // TUNGGU DALAM REALTIM
        }

        // Switch track
        musicSource.clip = nextTrack;
        musicSource.Play();

        // Fade IN new track
        percent = 0;
        while (percent < 1)
        {
            //percent += Time.deltaTime / fadeDuration;
            //musicSource.volume = Mathf.Lerp(0, 1, percent);
            //yield return null;
            percent += Time.unscaledDeltaTime / fadeDuration; // Gunakan unscaledDeltaTime
            musicSource.volume = Mathf.Lerp(0, 1, percent);
            yield return new WaitForSecondsRealtime(0f); // TUNGGU DALAM REALTIME
        }
    }
}
