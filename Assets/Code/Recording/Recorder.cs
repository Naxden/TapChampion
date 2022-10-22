using System.Collections;
using UnityEngine;
using Saving.SavingSystem;
using TMPro;
using UnityEngine.UI;

namespace Recording
{
    public class Recorder : MonoBehaviour
    {
#region Private Variables 
        string songPath, imagePath;
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        SpriteRenderer spriteRenderer;
        bool songLoaded = false;
        bool songIsPlaying = false;
#endregion
#region Control button
        [SerializeField, Header("Control buttons")]
        Button playButton;

        [SerializeField]
        Button pauseButton;

        [SerializeField]
        Button stopButton;

        [SerializeField]
        Slider slider;
#endregion
#region Song Timers
        [SerializeField, Header("Song timers")]
        TextMeshProUGUI currentTime;

        [SerializeField]
        TextMeshProUGUI songLength;
#endregion
        private void Awake()
        {
            FileManager.FileDialogInitialize();
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

        public void ImportSong()
        {
            if (songLoaded)
                StopSong();

            FileManager.SetDefaultFilter(FileManager.FileExtension.MUSIC);
            FileManager.ShowLoadDialog(LoadSongSucces, LoadSongCancel, "Load Song");
        }

        private void LoadSongCancel()
        {
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
            songLength.text = "/ " + GetFormattedTime(audioSource.clip.length);
            slider.maxValue = audioSource.clip.length;
            slider.interactable = true;
            songLoaded = true;
            
            playButton.gameObject.SetActive(true);
            playButton.interactable = true;
        }

        private string GetFormattedTime(float time)
        {
            int seconds = (int)time;

            return string.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
        }

        public void ImportImage()
        {
            FileManager.SetDefaultFilter(FileManager.FileExtension.IMAGE);
            FileManager.ShowLoadDialog(LoadImageSucces, LoadImageCancel, "Load Image");
        }

        private void LoadImageCancel()
        {
            spriteRenderer.sprite = null;
        }

        private void LoadImageSucces(string[] paths)
        {
            spriteRenderer.sprite = FileManager.GetSprite(paths[0]);
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

