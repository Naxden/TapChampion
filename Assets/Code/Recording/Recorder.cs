using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using Saving;
using System.Collections.Generic;
using System;

namespace Recording
{
    [System.Serializable]
    public class MyFloatEvent : UnityEvent<float> { }

    public class Recorder : MonoBehaviour
    {
#region References
        [SerializeField, Header("External References")]
        private NoteRenderer noteRenderer;
#endregion
#region Private Variables 

        private string songPath, imagePath;

        [SerializeField, Header("Local References")]
        private AudioSource audioSource;

        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public bool songLoaded { get; private set; } = false;
        public bool songIsPlaying { get; private set; } = false;
        private bool songExists = false;

        [SerializeField] 
        private UnityEvent OnSettingsLoad;

        [SerializeField]
        private MyFloatEvent OnSongLoad;
        
        [SerializeField]
        private UnityEvent OnSongSelect;

        [SerializeField]
        private NoteFile songNoteFile = new NoteFile();

        public UserSettings userSettings { get; private set; }
#endregion
#region Control button
        [SerializeField, Header("Control buttons")]
        private Button playButton;

        [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private Button stopButton;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Button importImageButton;
#endregion
#region Song Timers
        [SerializeField, Header("Song timers")]
        private TextMeshProUGUI currentTime;

        [SerializeField]
        private TextMeshProUGUI songLength;
#endregion
#region Song Input Data
        [SerializeField, Header("Song Input Data")]
        private TMP_InputField songTitleInput;

        [SerializeField]
        private TMP_InputField songAuthorInput;

        [SerializeField]
        private TMP_InputField songYearInput;
        #endregion

        private void Start()
        {
            UpdateKeyBinds();
        }

        private void Update()
        {
            if (songLoaded && songIsPlaying)
            {
                float timeElapsed = audioSource.time;
                currentTime.text = GetFormattedTime(timeElapsed);
                slider.value = timeElapsed;
            }
        }

        public void ChangePlaybackSpeed(int value)
        {
            switch (value)
            {
                case 0:
                    ChangeMixerSpeed(0.25f);
                    break;
                case 1:
                    ChangeMixerSpeed(0.5f);
                    break;
                case 2:
                    ChangeMixerSpeed(0.75f);
                    break;
                case 3:
                    ChangeMixerSpeed(1f);
                    break;
                case 4:
                    ChangeMixerSpeed(1.25f);
                    break;
                case 5:
                    ChangeMixerSpeed(1.5f);
                    break;
                case 6:
                    ChangeMixerSpeed(1.75f);
                    break;
                case 7:
                    ChangeMixerSpeed(2f);
                    break;
                default:
                    ChangeMixerSpeed(1f);
                    break;
            }
        }

        private void ChangeMixerSpeed(float speed)
        {
            audioSource.pitch = speed;
            if (speed >= 1f)
                audioMixer.SetFloat("MyPitch", 1f / speed);
            else
                audioMixer.SetFloat("MyPitch", 1f);
        }

        public void ImportSong()
        {
            if (songLoaded)
                StopSong();

            FileManager.ShowLoadDialog(LoadSongSucces, LoadSongCancel, "Load Song", FileManager.FileExtension.MUSIC);
            OnSongSelect.Invoke();
        }

        private void LoadSongCancel()
        {
            songPath = null;
            songLoaded = false;
            songIsPlaying = false;
            playButton.interactable = false;
            stopButton.interactable = false;
            slider.interactable = false;
        }

        private void LoadSongSucces(string[] paths)
        {
            StartCoroutine(LoadedSongRoutine(paths[0]));
        }

        private IEnumerator LoadedSongRoutine(string path)
        {
            yield return FileManager.GetAudioClipRoutine(path, audioSource);
            
            songPath = path;
            songNoteFile.title = FileManager.GetPartOfString(path, "\\", ".mp3");
            songExists = FileManager.DoesSongExist(songNoteFile.title);

            if (songExists)
                LoadExistingSong(songNoteFile.title);
            else
                ResetSongData();

            songLength.text = "/ " + GetFormattedTime(audioSource.clip.length);
            slider.maxValue = audioSource.clip.length;
            slider.interactable = true;
            songLoaded = true;
            
            playButton.gameObject.SetActive(true);
            playButton.interactable = true;
            
            OnSongLoad.Invoke(slider.maxValue);
        }

        private void LoadExistingSong(string songTitle)
        {
            string songDirPath = FileManager.GetPath("Songs/" + songTitle);

            spriteRenderer.sprite = FileManager.GetSprite(songDirPath + $"/{songTitle}.png");
            
            songNoteFile = FileManager.GetNoteFile(songDirPath + $"/{songTitle}.note");
            songTitleInput.text = songNoteFile.title;
            songAuthorInput.text = songNoteFile.author;
            songYearInput.text = songNoteFile.year.ToString();
        }

        private void ResetSongData()
        {
            songNoteFile = new NoteFile();

            songTitleInput.text = "";
            songAuthorInput.text = "";
            songYearInput.text =  "";
        }

        private string GetFormattedTime(float time)
        {
            int seconds = (int)time;

            return string.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
        }

        // function called by OnSongLoad Event
        public void RenderLoadedNotes()
        {
            float[] trackPositions = noteRenderer.GetTrackPositions();
            List<NoteObject> notes;

            if (userSettings.difficulty == (int)Difficulty.EASY)
                notes = songNoteFile.easy;    
            else if (userSettings.difficulty == (int)Difficulty.MEDIUM)
                notes = songNoteFile.medium;
            else
                notes = songNoteFile.hard;

            foreach (NoteObject note in notes)
            {
                Vector3 notePosition = new Vector3(note.spawnTime * 7.5f - 5f,
                                                    trackPositions[note.buttonIndex],
                                                    0f);

                noteRenderer.TryAddNote(notePosition, (NoteType)note.noteType);
            }
        }

        public void UpdateKeyBinds()
        {
            userSettings = FileManager.GetUserSettings();

            OnSettingsLoad.Invoke();
        }

        // function called by OnDifficultyChanged Event
        public void LoadDifferentDifficulty()
        {
            if (!songLoaded)
                return;

            userSettings = FileManager.GetUserSettings();

            noteRenderer.ClearTracks();

            RenderLoadedNotes();
        }

        // function called by ButtonClick
        public void SaveSong()
        {
            if (!songLoaded)
            {
                Debug.LogWarning("SaveSong: no song loaded");
                return;
            }
            if (songNoteFile == null)
            {
                Debug.LogWarning("SaveSong: noteFile is empty");
                return;
            }

            songNoteFile.title = songTitleInput.text;
            songNoteFile.author = songAuthorInput.text;
            songNoteFile.year = Convert.ToInt32(songYearInput.text);

            List<NoteObject> noteMap = noteRenderer.GetSortedNoteMap();

            if (userSettings.difficulty == (int)Difficulty.EASY)
                songNoteFile.easy = noteMap;
            else if (userSettings.difficulty == (int)Difficulty.MEDIUM)
                songNoteFile.medium = noteMap;
            else
                songNoteFile.hard = noteMap;

            FileManager.RecordSong(songNoteFile.title, 
                                   songNoteFile, 
                                   imagePath,
                                   songExists ? null : songPath);
        }

        public void ImportImage()
        {
            FileManager.ShowLoadDialog(LoadImageSucces, LoadImageCancel, 
                                      "Load Image", FileManager.FileExtension.IMAGE);
        }

        private void LoadImageCancel()
        {
            spriteRenderer.sprite = null;
            imagePath = null;
        }

        private void LoadImageSucces(string[] paths)
        {
            spriteRenderer.sprite = FileManager.GetSprite(paths[0]);
            imagePath = paths[0];
        }

        public void PlaySong()
        {
            audioSource.Play();
            songIsPlaying = true;
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            stopButton.interactable = true;
        }

        public void PauseSong()
        {
            if (!songLoaded)
                return;

            audioSource.Pause();
            songIsPlaying = false;
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            stopButton.interactable = true;
        }

        public void StopSong()
        {
            slider.value = 0f;
            audioSource.Pause();
            audioSource.time = 0f;
            songIsPlaying = false;
            
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            stopButton.interactable = false;

            currentTime.text = GetFormattedTime(0f);
        }

        public void UpdateSongTime()
        {
            songIsPlaying = false;
            float timeElapsed = slider.value;
            audioSource.time = timeElapsed;
            currentTime.text = GetFormattedTime(timeElapsed);
        }

    }
}

