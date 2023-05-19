using Saving;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Settings
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private SettingsManager settingsManager;

        private float musicVolume;
        private float sfxVolume;
        private bool volumeChanged = false;

        [SerializeField]
        private Slider musicSlider;
        [SerializeField]
        private Slider sfxSlider;

        [SerializeField]
        private AudioMixer audioMixer;


        public void InitializeSounds(float musicVolume, float sfxVolume)
        {
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
            volumeChanged = false;
        }

        private void Awake()
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        private void OnEnable()
        {
            musicVolume = settingsManager.GetMusicVolume();
            sfxVolume = settingsManager.GetSFXVolume();
            musicSlider.value = musicVolume;
            sfxSlider.value = sfxVolume;

            volumeChanged = false;
        }

        private void Update()
        {
            if (volumeChanged)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    settingsManager.SetMusicVolume(musicSlider.value);
                    settingsManager.SetSFXVolume(sfxSlider.value);
                    settingsManager.SaveSettings();

                    volumeChanged = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
                ResetToDefault();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (volumeChanged)
                {
                    SetMusicVolume(musicVolume);
                    SetSFXVolume(sfxVolume);
                }

                gameObject.SetActive(false);
                settingsManager.ShowMainSettings();
            }
        }

        private void SetMusicVolume(float volume)
        {
            volumeChanged = true;
            audioMixer.SetFloat("MyMusicVolume", Mathf.Log10(volume) * 20);
        }

        private void SetSFXVolume(float volume)
        {
            volumeChanged = true;
            audioMixer.SetFloat("MySFXVolume", Mathf.Log10(volume) * 20);
        }

        private void ResetToDefault()
        {
            musicSlider.value = 1f;
            sfxSlider.value = 1f;

            volumeChanged = true;
            // SetFunctions will be called throug Event Subscriptions
        }
    }
}