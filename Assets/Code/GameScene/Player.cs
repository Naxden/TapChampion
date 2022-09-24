using UnityEngine;
using GameScene.UI.UIController;
using UnityEditor.PackageManager;

namespace GameScene.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        UIController uIController;

        [SerializeField]
        int score, hitNotesCounter, missedNotesCounter, multiplier; 
        
        public void NoteWasHit(string hitInfo)
        {
            multiplier = 2;
            score += multiplier;
            hitNotesCounter++;

            uIController.UpdateInfo(score, 
                                    hitInfo, 
                                    CalculateAccuracy(), 
                                    multiplier);
        }
        public void NoteWasMissed()
        {
            missedNotesCounter++;
            multiplier = 1;
            uIController.UpdateInfo(score,
                                    "Missed",
                                    CalculateAccuracy(),
                                    multiplier);
        }
        private int CalculateAccuracy()
        {
            return (int)(100f * hitNotesCounter / (hitNotesCounter + missedNotesCounter));
        }
    }
}

