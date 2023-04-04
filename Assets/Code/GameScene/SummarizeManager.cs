using Global;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameScene
{
    public class SummarizeManager : MonoBehaviour
    {
        [SerializeField, Header("External")]
        FadeManger fadeManger;

        [SerializeField]
        Player player;

        [SerializeField, Header("Internal")]
        GameObject summarizeContent;

        [SerializeField, Header("Text Values")]
        private TextMeshProUGUI scoreTMP;
        [SerializeField]
        private TextMeshProUGUI accuracyTMP;
        [SerializeField]
        private TextMeshProUGUI notesMissedTMP;

        public void Summarize(int score, int notesHit, int notesMissed)
        {
            scoreTMP.text = score.ToString();

            float accuracy = 100f * notesHit / (notesHit + notesMissed);
            if (accuracy == float.NaN)
                accuracy = 0f;

            accuracyTMP.text = $"{accuracy:0.00} %";

            notesMissedTMP.text = notesMissed.ToString();

            summarizeContent.SetActive(true);
        }
    }
}
