using UnityEngine;
using GameScene.UI.UIController;
using UnityEditor.PackageManager;

namespace GameScene.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        UIController uiController;

        [SerializeField]
        NoteManager noteManager;

        [SerializeField]
        int score, hitNotesCounter, missedNotesCounter, multiplier;

        NoteFile noteFile;

        List<NoteObject> noteObjects;
        
        public void NoteWasHit(string hitInfo)
        {
            multiplier = 2;
            score += multiplier;
            hitNotesCounter++;

            uiController.UpdateInfo(score, 
                                    hitInfo, 
                                    CalculateAccuracy(), 
                                    multiplier);
        }

        public void LongNoteBeingHit()
        {
            score += multiplier;

            uiController.UpdateScore(score);
        }

        public void NoteWasMissed()
        {
            missedNotesCounter++;
            multiplier = 1;
            uiController.UpdateInfo(score,
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

