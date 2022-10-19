using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving.SavingSystem;
using TMPro;

namespace Recording
{
    public class Recorder : MonoBehaviour
    {
        string songPath;
        [SerializeField]
        AudioSource audioSource;
        bool songLoaded = false;

        [SerializeField, Header("Control buttons")]
        GameObject playButton;
        [SerializeField]
        GameObject pauseButton;
        [SerializeField]
        GameObject stopButton;

        [SerializeField, Header("Song timers")]
        TextMeshProUGUI currentTime;
        [SerializeField]
        TextMeshProUGUI songLength;

        private void Start()
        {
            FileManager.FileDialogInitialize();
            FileManager.SetDefaultFilter(FileManager.FileExtension.MUSIC);
            FileManager.ShowLoadDialog(OnSucces, OnCancel, "Load Song");
        }

        private void Update()
        {
            if (songLoaded)
            {
                currentTime.text = GetFormattedTime(audioSource.time);
            }
        }

        private void OnSucces(string[] paths)
        {
            StartCoroutine(LoadedSongRoutine(paths[0]));
        }

        private IEnumerator LoadedSongRoutine(string path)
        {
            yield return FileManager.GetAudioClipRoutine(path, audioSource);

            songLength.text = "/ "+GetFormattedTime(audioSource.clip.length);
            playButton.SetActive(true);
            songLoaded = true;
        }

        private string GetFormattedTime(float time)
        {
            int seconds = (int)time;
            return string.Format("{0}:{1:D2}",seconds / 60, seconds % 60);
        }

        public void PlaySong()
        {
            audioSource.Play();
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            stopButton.SetActive(true);
        }

        public void PauseSong()
        {
            audioSource.Pause();
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            stopButton.SetActive(true);
        }

        public void StopSong()
        {
            audioSource.Stop();
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            stopButton.SetActive(false);
        }

        private void OnCancel()
        {
            Debug.Log("Przerwano dialog");
        }
    }
}

