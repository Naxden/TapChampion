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

        private bool LagChanged = false;

        private void OnEnable()
        {
            userLag = settingsManager.GetUserLag();
        }

        void Update()
        {
            if (LagChanged)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    settingsManager.SetUserLag(userLag);
                    settingsManager.SaveSettings();
                    settingsManager.DifficultyChanged();

                    LagChanged = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
                ResetToDefault();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                settingsManager.EnableMainSettings();
            }
        }

        public void IncreseLag()
        {
            userLag += 0.25f;
            LagChanged = true;
            UpdateText();
        }

        public void DecreaseLag()
        {
            userLag -= 0.25f;
            LagChanged = true;
            UpdateText();
        }

        private void UpdateText()
        {
            userLagText.text = string.Format("{0:0.00}", userLag.ToString());
        }

        private void ResetToDefault()
        {
            userLag = 0f;
            LagChanged = true;
            UpdateText();
        }
    }

}
