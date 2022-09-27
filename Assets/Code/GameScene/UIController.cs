using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene.Player;
using UnityEngine.UI;
using TMPro;

namespace GameScene.UI.UIController
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI scoreTM, hitInfoTM, accuracyTM, multiplierTM;

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
    }
}

