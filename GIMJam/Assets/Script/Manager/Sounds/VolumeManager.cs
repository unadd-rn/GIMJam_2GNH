using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusic("Main BG");
    }

    float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f)
            return -80f; // silent
        return Mathf.Log10(linear) * 20f;
    }

    public void UpdateMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDecibel(sliderValue));
    }

    public void UpdateMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(sliderValue));
    }

    public void UpdateSoundVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFXVolume", LinearToDecibel(sliderValue));
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void explosion()
    {
        SoundManager.Instance.PlaySound2D("explosion");
    }

    public void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value  = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value    = PlayerPrefs.GetFloat("SFXVolume", 1f);

        UpdateMasterVolume(masterSlider.value);
        UpdateMusicVolume(musicSlider.value);
        UpdateSoundVolume(sfxSlider.value);
    }
}
