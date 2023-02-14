using UnityEngine;
using TMPro;

namespace GameScene
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI scoreTM, hitInfoTM, accuracyTM, multiplierTM, timerTM;

        public void UpdateInfo(int score, string hitInfo, int accuracy, int multiplier)
        {
            scoreTM.text = $"Score: {score}";
            hitInfoTM.text = hitInfo;
            accuracyTM.text = $"Accuracy: {accuracy}%";
            multiplierTM.text = $"x{multiplier}";
        }

        public void UpdateScore(int score)
        {
            scoreTM.text = $"Score: {score}";
        }

        public void UpdateTime(float time)
        {
            timerTM.text = $" {time}";
        }
    }
}

