using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Saving;
using SongSelect;
using Global;
using Settings;
using UnityEditor.PackageManager;

namespace GameScene
{
    public class Player : MonoBehaviour
    {
    #region Externals
        [Header("External Links")]
        [SerializeField]
        UIController uiController;

        [SerializeField]
        NoteManager noteManager;

        [SerializeField]
        SettingsManager settingsManager;

        [SerializeField]
        private FadeManger fadeManger;

        [SerializeField]
        private SummarizeManager summarizeManager;

        [SerializeField]
        private CountDownManager countDownManager;

        [SerializeField]
        private Button[] buttons;

        [SerializeField]
        private UnityEngine.UI.Button[] nextSongButtons;
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
        
        [SerializeField, Header("Song")]
        AudioSource audioSource;

        [SerializeField]
        SpriteRenderer songBackgroundRenderer;
        #endregion

        #region PlayerSettings
        private UserSettings userSettings;
        #endregion


        private void Awake()
        {
            LoadSettings();
            SetButtonsKeys();
            SetDifficulty();
            noteManager.SetUserLag(userSettings.userLag);
        }

        public void LoadSettings()
        {
            userSettings = FileManager.GetUserSettings();
        }

        public void SetButtonsKeys()
        {
            for (int i = (int)GameKeys.BUTTON1; i < (int)GameKeys.BUTTON5; i++)
            {
                buttons[i].SetKey((KeyCode)userSettings.keys[i]);
            }
        }

        public void SetDifficulty()
        {
            difficulty = (Difficulty)userSettings.difficulty;
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

            if (timerStarted && gameTimer >= songLength)
            {
                FinishSong();
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

        private float CalculateAccuracy()
        {
            return (100f * hitNotesCounter / (hitNotesCounter + missedNotesCounter));
        }

        public void LoadSong()
        {
            song = SongManager.songsToPlay[0];
            SongManager.songsToPlay.RemoveAt(0);

            if (SongManager.songsToPlay.Count == 0)
            {
                foreach (var button in nextSongButtons)
                {
                    button.interactable = false;
                }
            }

            songBackgroundRenderer.sprite = song.noteSprite;

            audioSource.clip = song.noteAudioClip;
            songLength = audioSource.clip.length;

            noteManager.Intialize(GetNoteList());
            ResetGameNumbers();

            noteManager.SetTimer(gameTimer);
            PlaySong();
        }

        private void ResetGameNumbers()
        {
            gameTimer = -3f;
            score = 0;
            hitNotesCounter = 0;
            missedNotesCounter = 0;
            multiplier = 1;
        }

        public void PlaySong()
        {
            StartCoroutine(PlaySongRoutine());
        }

        private IEnumerator PlaySongRoutine()
        {
            yield return fadeManger.FadeRoutine(false);

            timerStarted = true;
            noteManager.SongIsPlaying(true);
            noteManager.SpawnNotes(true);

            settingsManager.EnableInput(false);
            yield return countDownManager.CountDownRoutine();
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
            settingsManager.EnableInput(false);
            yield return countDownManager.CountDownRoutine();
            settingsManager.EnableInput(true);

            audioSource.Play();

            noteManager.SongIsPlaying(true);

            timerStarted = true;
        }

        //Change difficulty

        //Finish Song
        private void FinishSong()
        {
            audioSource.Stop();
            timerStarted = false;
            noteManager.SongIsPlaying(false);

            summarizeManager.Summarize(score, hitNotesCounter, missedNotesCounter);
            SaveSongScore();
        }

        private void SaveSongScore()
        {
            float accuracy = CalculateAccuracy();
            bool isNewRecord = score > song.noteFile.highScores[(int)difficulty] || 
                               accuracy > song.noteFile.accuracies[(int)difficulty];
            
            if (!isNewRecord)
                return;

            string title = song.noteFile.title;

            song.noteFile.highScores[(int)difficulty] = score;
            song.noteFile.accuracies[(int)difficulty] = accuracy;

            FileManager.RecordSong(title, song.noteFile, null, null);
        }

        public void NextSong()
        {
            StartCoroutine(NextSongRoutine());
        }

        private IEnumerator NextSongRoutine()
        {
            yield return fadeManger.FadeRoutine(true);

            FreshStart();

            LoadSong();
        }

        //Restart Song is called by Buttons
        //and only if song is NOT playing And Fader is On
        public void RestartSong()
        {
            StartCoroutine(RestartSongRoutine());
        }

        private IEnumerator RestartSongRoutine()
        {
            yield return fadeManger.FadeRoutine(true);

            FreshStart();

            PlaySong();
        }

        private void FreshStart()
        {
            audioSource.time = 0;

            ResetGameNumbers();
            uiController.UpdateInfo(score, "", 0f, multiplier);

            foreach (Button button in buttons)
            {
                button.ClearNotesQueue();
            }

            noteManager.ResetSong();
            noteManager.SetTimer(gameTimer);
        }

        // Function called by the Exit button
        public void ClearSongsQueue()
        {
            SongManager.songsToPlay.Clear();
        }


    }
}

