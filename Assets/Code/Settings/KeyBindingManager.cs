using Saving;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Settings
{
    public class KeyBindingManager : MonoBehaviour
    {
        [SerializeField]
        private SettingsManager settingsManager;

        [Header("Keys reference"), SerializeField]
        private TMP_Text[] keysTMP;
        
        [SerializeField]
        private GameObject inputPrompt;
        [SerializeField]
        private GameObject resetPrompt;
        private bool resetToDefault = false;

        private readonly List<int> defaultKeyCodes = new List<int>()
        {
            (int)KeyCode.D, (int)KeyCode.F, (int)KeyCode.J, (int)KeyCode.K, (int)KeyCode.L,

            (int)KeyCode.Delete,(int)KeyCode.Space
        };
        private List<int> userKeyCodes;


        private KeyCode newKey;
        private bool settingNewKey = false;
        private bool keyChanged = false;

        private void OnEnable()
        {
            userKeyCodes = settingsManager.GetKeys();

            UpdateKeysText();
        }

        private void UpdateKeysText()
        {
            for (int i = 0; i < userKeyCodes.Count; i++)
            {
                keysTMP[i].text = ((KeyCode)userKeyCodes[i]).ToString();
            }
        }

        private void Update()
        {
            if (!settingNewKey)
            {
                if (Input.GetKeyDown(KeyCode.Return) && keyChanged)
                {
                    settingsManager.SetKeys(userKeyCodes);
                    settingsManager.SaveSettings();
                    settingsManager.KeyBindsChanged();

                    keyChanged = false;
                }
                if (Input.GetKeyDown(KeyCode.X))
                    StartCoroutine(ResetSettingsRoutine());
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    gameObject.SetActive(false);
                    settingsManager.EnableInput();
                }
            }
        }

        private IEnumerator ResetSettingsRoutine()
        {
            resetPrompt.SetActive(true);

            yield return new WaitUntil(() => !resetPrompt.activeSelf);

            if (!resetToDefault)
            {
                yield break;
            }

            userKeyCodes = defaultKeyCodes;
            UpdateKeysText();
            resetToDefault = false;
            keyChanged = true;
        }

        // Function called by Event on resetPrompt
        public void ConfirmReset()
        {
            resetToDefault = true;
        }

        public void SetKey(int keyIndex)
        {
            StartCoroutine(SetKeyRoutine(keyIndex));
        }

        private IEnumerator SetKeyRoutine(int keyIndex)
        {
            settingNewKey = true;
            inputPrompt.SetActive(true);

            yield return CaptureInputRoutine();
            inputPrompt.SetActive(false);

            if (!ValidateNewKeyValue(keyIndex))
            {
                settingNewKey = false;
                yield break;
            }

            userKeyCodes[keyIndex] = (int)newKey;

            keysTMP[keyIndex].text = newKey.ToString();

            settingNewKey = false;
            keyChanged = true;
        }

        IEnumerator CaptureInputRoutine()
        {
            do
            {
                newKey = GetKey();

                yield return null;
            } while (newKey == KeyCode.None);
        }

        private KeyCode GetKey()
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {                
                if (Input.GetKey(key)) 
                    return key;
            }

            return KeyCode.None;
        }

        // In future
        // possible same keyCode for different buttons on separate scenes
        private bool ValidateNewKeyValue(int keyIndex)
        {
            if (Input.GetKey(KeyCode.None)) return false;
            if (Input.GetKey(KeyCode.Return)) return false;
            if (Input.GetKey(KeyCode.Escape)) return false;
            if (Input.GetKey(KeyCode.Mouse0)) return false;
            if (Input.GetKey(KeyCode.LeftWindows)) return false;
            if (Input.GetKey(KeyCode.RightWindows)) return false;

            foreach (int key in userKeyCodes)
            {
                if (key == (int)newKey)
                    return false;
            }

            return true;
        }
    }
}
