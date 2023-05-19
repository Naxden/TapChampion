using Global;
using Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private bool inSecondarySettings = false;
        private bool inputEnabled = false;

        [SerializeField]
        GameObject settingsPanel;
        [SerializeField]
        GameObject settingsContent;

        [SerializeField]
        SoundManager soundManager;

        private UserSettings userSettings;

        public UnityEvent OnAppear;
        public UnityEvent OnCalibrationChanged; 
        public UnityEvent OnDifficultyChanged; 
        public UnityEvent OnKeyBindsChanged;
        public UnityEvent OnDisappear;

        private bool calibrationChanged = false;
        private bool difficultyChanged = false;
        private bool keyBindsChanged = false;

        private void Awake()
        {
            userSettings = FileManager.GetUserSettings();
        }

        private void Start()
        {
            soundManager.InitializeSounds(userSettings.musicVolume, userSettings.sfxVolume);
        }

        private void Update()
        {
            if (inputEnabled && !inSecondarySettings)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!settingsPanel.activeSelf)
                        OpenSettings();
                    else
                        CloseSettings();
                }
            }
        }

        public void OpenSettings()
        {
            settingsPanel.SetActive(true);

            OnAppear.Invoke();
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);

            OnDisappear.Invoke();
        }

        private void InvokeManagerEvents()
        {
            if (calibrationChanged)
            {
                calibrationChanged = false;
                OnCalibrationChanged.Invoke();
            }

            if (keyBindsChanged)
            {
                keyBindsChanged = false;
                OnKeyBindsChanged.Invoke();
            }

            if (difficultyChanged)
            {
                difficultyChanged = false;
                OnDifficultyChanged.Invoke();
            }
        }

        public void EnableInput(bool toEnable)
        {
            inputEnabled = toEnable;
        }

        public void SaveSettings()
        {
            FileManager.WriteUserSettings(userSettings);
        }

        public void ShowMainSettings()
        {
            if (inSecondarySettings)
                InvokeManagerEvents();

            inSecondarySettings = false;
            settingsContent.SetActive(true);
        }

        public float GetUserLag()
        {
            return userSettings.userLag;
        }

        public void SetUserLag(float userLag)
        {
            userSettings.userLag = userLag;
        }

        public int GetDifficulty()
        {
            return userSettings.difficulty;
        }

        public void SetDifficulty(int difficulty)
        {
            userSettings.difficulty = difficulty;
        }

        public float GetMusicVolume()
        {
            return userSettings.musicVolume;
        }

        public void SetMusicVolume(float musicVolume)
        {
            userSettings.musicVolume = musicVolume;
        }

        public float GetSFXVolume()
        {
            return userSettings.sfxVolume;
        }

        public void SetSFXVolume(float sfxVolume)
        {
            userSettings.sfxVolume = sfxVolume;
        }

        public List<int> GetKeys()
        {
            return userSettings.keys;
        }

        public void SetKeys(List<int> keys)
        {
            userSettings.keys = keys;
        }

        // function called by Events on SettingsButtons
        public void HideMainSettings()
        {
            inSecondarySettings = true;
            settingsContent.SetActive(false);
        }

        public void CalibrationChanged()
        {
            calibrationChanged = true;
        }

        public void DifficultyChanged()
        {
            difficultyChanged = true;
        }

        public void KeyBindsChanged()
        {
            keyBindsChanged = true;
        }
    }
}
