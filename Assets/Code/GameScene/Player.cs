using UnityEngine;
using GameScene.UI.UIController;
using Saving.Note;
using System.Collections.Generic;
using GameScene.Notes.NoteManager;
using System.Collections;
using Saving.SavingSystem;

namespace GameScene.Player
{
    public class Player : MonoBehaviour
    {
    #region Managers
        [Header("Managers Links")]
        [SerializeField]
        UIController uiController;

        [SerializeField]
        NoteManager noteManager;
    #endregion

    #region gameNumbers
        [Header("GameplayValues")]
        [SerializeField]
        int score;
        [SerializeField]
        int hitNotesCounter;
        [SerializeField]
        int missedNotesCounter;
        [SerializeField]
        int multiplier;
    #endregion

    #region SongVariables
        string songName;
        NoteFile noteFile;
        Difficulty difficulty;
        [SerializeField, Header("Song")]
        AudioSource audioSource;
        [SerializeField]
        SpriteRenderer songBackgroundRenderer;
        #endregion

    #region PlayerSettings
        [SerializeField, Header("Player Settings")]
        float timeCalibration = 0f;
    #endregion

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


        private void Start()
        {
            StartCoroutine(SongInitializeRoutine());
        }

        IEnumerator SongInitializeRoutine()
        {
            songName = "Test";

            songBackgroundRenderer.sprite = FileManager.GetSprite(songName);

            yield return FileManager.GetAudioClipRoutine(songName, audioSource);

            noteFile = FileManager.GetNoteFile(songName);
            noteManager.Intialize(GetNoteList(), timeCalibration);
        }

        private List<NoteObject> GetNoteList()
        {
            if (noteFile != null)
            {
                switch((int)difficulty)
                {
                    case 0:
                        return noteFile.easy;
                    case 1:
                        return noteFile.medium;
                    case 2:
                        return noteFile.hard;
                    default:
                        Debug.Log("Player.GetNotes() difficulty is not set");
                        return null;
                }
            }

            Debug.LogWarning("Player.GetNotes() noteFile is empty");
            return null;    
        }

        private int CalculateAccuracy()
        {
            return (int)(100f * hitNotesCounter / (hitNotesCounter + missedNotesCounter));
        }
    }
}

