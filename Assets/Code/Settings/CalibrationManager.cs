using TMPro;
using UnityEngine;


namespace Settings
{
    public class CalibrationManager : MonoBehaviour
    {

        [SerializeField]
        private SettingsManager settingsManager;

        [SerializeField]
        private TMP_Text userLagText;

        private float userLag;

        private bool lagChanged = false;

        private void OnEnable()
        {
            userLag = settingsManager.GetUserLag();
        }

        private void Update()
        {
            if (lagChanged)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    settingsManager.SetUserLag(userLag);
                    settingsManager.SaveSettings();
                    settingsManager.PlaySaveSound();
                    settingsManager.CalibrationChanged();

                    lagChanged = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
                ResetToDefault();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                settingsManager.ShowMainSettings();
            }
        }

        public void IncreseLag()
        {
            userLag += 0.1f;
            lagChanged = true;
            UpdateText();
        }

        public void DecreaseLag()
        {
            userLag -= 0.1f;
            lagChanged = true;
            UpdateText();
        }

        private void UpdateText()
        {
            userLagText.text = $"{userLag:0.0}";
        }

        private void ResetToDefault()
        {
            userLag = 0f;
            lagChanged = true;
            UpdateText();
        }
    }

}
