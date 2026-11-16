using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    void Start()
    {
        SetSliders();
    }

    void SetSliders()
    {
        // Slider values: [0.0001, 1]
        // Mixer values: [-80, 0]
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void UpdateMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", ConvertSliderValueToVolumeLevel(masterVolumeSlider.value));
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
    }

    public void UpdateMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", ConvertSliderValueToVolumeLevel(musicVolumeSlider.value));
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void UpdateSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", ConvertSliderValueToVolumeLevel(sfxVolumeSlider.value));
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }

    float ConvertSliderValueToVolumeLevel(float sliderValue)
    {
        return Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
    }
}
