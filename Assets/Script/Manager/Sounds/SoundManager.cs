using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioMixer audioMixer;

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

            // Mixer handles loudness — leave AudioSource at full volume
            sfx2DSource.volume = 1f;
        }
    }

    public void SetPitch(float pitch)
    {
        sfx2DSource.pitch = pitch;
    }


    public void PlaySound2D(string soundName)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            sfx2DSource.PlayOneShot(clip);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("3D Sound");
        temp.transform.position = pos;

        AudioSource src = temp.AddComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.clip = clip;
        src.outputAudioMixerGroup = sfx2DSource.outputAudioMixerGroup; // route to SFX
        src.volume = 1f;

        src.Play();
        Destroy(temp, clip.length);
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }
}

