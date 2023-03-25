using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Saving;
using SongSelect;
using Global;
using System.Runtime.InteropServices;
using Settings;

namespace GameScene
{
    public class Player : MonoBehaviour
    {
    #region Managers
        [Header("Managers Links")]
        [SerializeField]
        UIController uiController;

        [SerializeField]
        NoteManager noteManager;

        [SerializeField]
        SettingsManager settingsManager;

        [SerializeField]
        private FadeManger fadeManger;

        [SerializeField]
        private Button[] buttons;
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

        [SerializeField]
        private float gameTimer = -3f;
        private bool timerStarted = false;
        #endregion

        #region SongVariables
        private Song song;
        Difficulty difficulty = Difficulty.MEDIUM;
        private float songLength = 0f;
        private bool songFinished = false;
        
        [SerializeField, Header("Song")]
        AudioSource audioSource;

        [SerializeField]
        SpriteRenderer songBackgroundRenderer;
        #endregion

        #region PlayerSettings
        private UserSettings userSettings;
        float userLag = -0.06f;
        #endregion


        private void Awake()
        {
            userSettings = FileManager.GetUserSettings();

            difficulty = (Difficulty)userSettings.difficulty;
            LoadButtonsKeys();
        }

        private void LoadButtonsKeys()
        {
            for (int i = (int)GameKeys.BUTTON1; i < (int)GameKeys.BUTTON5; i++)
            {
                buttons[i].SetKey((KeyCode)userSettings.keys[i]);
            }
        }

        private void Start()
        {
            LoadSong();
        }


        private void Update()
        {
            if (timerStarted && gameTimer <= songLength)
            {
                gameTimer += Time.deltaTime;

                noteManager.SetTimer(gameTimer);
                uiController.UpdateTime(gameTimer);
            }
        }

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

        private void LoadSong()
        {
            song = SongManager.songsToPlay[0];
            SongManager.songsToPlay.RemoveAt(0);

            songBackgroundRenderer.sprite = song.noteSprite;

            audioSource.clip = song.noteAudioClip;
            songLength = audioSource.clip.length;

            noteManager.Intialize(GetNoteList(), userLag);
            
            noteManager.SetTimer(gameTimer);
            PlaySong();
        }

        public void PlaySong()
        {
            StartCoroutine(PlaySongRoutine());
        }

        private IEnumerator PlaySongRoutine()
        {
            settingsManager.EnableInput(false);

            yield return fadeManger.FadeRoutine(false);

            timerStarted = true;
            noteManager.SongIsPlaying(true);

            yield return new WaitForSeconds(3f);

            settingsManager.EnableInput(true);

            audioSource.Play();
        }

        private List<NoteObject> GetNoteList()
        {
            if (song.noteFile == null)
            {
                Debug.LogWarning("Player.GetNotes() noteFile is empty");
                return null;
            }

            if (difficulty == Difficulty.EASY) 
                return song.noteFile.easy;
            if (difficulty == Difficulty.MEDIUM) 
                return song.noteFile.medium;
            if (difficulty == Difficulty.HARD) 
                return song.noteFile.hard;

            Debug.Log("Player.GetNotes() difficulty is not set");
            return null;
        }

        //Pause Song
        public void PauseSong()
        {
            audioSource.Pause();

            timerStarted = false;
            noteManager.SongIsPlaying(false);
        }

        //Resume song
        public void ResumeSong()
        {
            StopCoroutine(ResumeSongRoutine());
            StartCoroutine(ResumeSongRoutine());
        }

        private IEnumerator ResumeSongRoutine()
        {
            Debug.Log("timer started");
            settingsManager.EnableInput(false);
            //countdown
            yield return new WaitForSeconds(3f);

            settingsManager.EnableInput(true);

            audioSource.Play();

            noteManager.SongIsPlaying(true);

            timerStarted = true;
        }

        //Change difficulty

        //Finish Song

        //Restart Song




    }
}

