using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class DifficultyManager : MonoBehaviour
    {
        [SerializeField]
        private SettingsManager settingsManager;

        private int difficulty;

        private bool difficultyChanged = false;

        [SerializeField]
        private Button[] modeButtons;

        private void OnEnable()
        {
            difficulty = settingsManager.GetDifficulty();

            foreach (Button button in modeButtons)
            {
                button.interactable = true;
            }

            modeButtons[difficulty].interactable = false;
        }

        private void Update()
        {
            if (difficultyChanged)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    settingsManager.SetDifficulty(difficulty);
                    settingsManager.SaveSettings();
                    settingsManager.PlaySaveSound();
                    settingsManager.DifficultyChanged();

                    difficultyChanged = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                settingsManager.ShowMainSettings();
            }
        }

        public void SetDifficulty(int difficulty)
        {
            modeButtons[this.difficulty].interactable = true;

            this.difficulty = difficulty;

            modeButtons[this.difficulty].interactable = false;

            difficultyChanged = true;
        }
    }
}
