using Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private bool isVisible = false;

        [SerializeField]
        GameObject settingsPanel;
        [SerializeField]
        GameObject settingsContent;

        private UserSettings userSettings;

        public UnityEvent OnAppear;
        public UnityEvent OnCalibrationChanged; 
        public UnityEvent OnDifficultyChanged; 
        public UnityEvent OnKeyBindsChanged;
        public UnityEvent OnDisappear;

        private bool calibrationChanged = false;
        private bool difficultyChanged = false;
        private bool keyBindsChanged = false;

        private void OnEnable()
        {
            isVisible = true;
            settingsContent.SetActive(true);
            userSettings = FileManager.GetUserSettings();
        }

        private void Update()
        {
            if (isVisible)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!settingsPanel.activeSelf)
                        OnAppear.Invoke();
                    else
                    {
                        InvokeManagerEvents();

                        OnDisappear.Invoke();
                    }
                }
            }
        }

        private void InvokeManagerEvents()
        {
            if (calibrationChanged)
            {
                calibrationChanged = false;
                OnCalibrationChanged.Invoke();
            }

            if (difficultyChanged)
            {
                difficultyChanged = false;
                OnDifficultyChanged.Invoke();
            }

            if (keyBindsChanged)
            {
                keyBindsChanged = false;
                OnKeyBindsChanged.Invoke();
            }
        }

        public void SaveSettings()
        {
            FileManager.WriteUserSettings(userSettings);
        }

        public void EnableInput()
        {
            isVisible = true;
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

        public List<int> GetKeys()
        {
            return userSettings.keys;
        }

        public void SetKeys(List<int> keys)
        {
            userSettings.keys = keys;
        }

        // function called by Events on SettingsButtons
        public void DisableInput()
        {
            isVisible = false;
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
