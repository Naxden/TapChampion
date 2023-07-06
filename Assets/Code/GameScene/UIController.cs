using UnityEngine;
using TMPro;

namespace GameScene
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreTM, hitInfoTM, accuracyTM, multiplierTM;

        public void UpdateInfo(int score, string hitInfo, float accuracy, int multiplier)
        {
            scoreTM.text = $"Score: {score}";
            hitInfoTM.text = hitInfo;
            accuracyTM.text = $"Accuracy: {accuracy:0.00} %";
            multiplierTM.text = $"x{multiplier}";
        }

        public void UpdateScore(int score)
        {
            scoreTM.text = $"Score: {score}";
        }
    }
}

