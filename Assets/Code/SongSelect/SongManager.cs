using Global;
using Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SongSelect
{
    public class SongManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject songsHolder;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private GameObject songPrefab;

        [SerializeField]
        private FadeManger fadeManger;

        private Vector3 spawnPosition = new Vector3(0f, 200f, 0f);
        private List<Song> loadedSongs = new List<Song>();
        public static List<Song> songsToPlay = new List<Song>();

        private Difficulty difficulty;

        private const float startVolume = 0.1f;
        float endVolume = 1.0f;
        [SerializeField]
        float songEffectDuration = 0.8f;

        private string songToExport;
        [SerializeField]
        private GameObject exportPrompt;

        private void Start()
        {
            if (songsToPlay.Count > 0)
                songsToPlay.Clear();

            UpdateDifficulty();
            StartCoroutine(LoadSongsRoutine());
        }

        private IEnumerator LoadSongsRoutine()
        {
            string[] paths = FileManager.GetAllSongs();

            foreach (string path in paths)
            {
                yield return InstantiateSongRoutine(path);
            }

            yield return fadeManger.FadeRoutine(false);
        }
        private IEnumerator InstantiateSongRoutine(string songDirPath)
        {
            string songTitle = FileManager.GetPartOfString(songDirPath, "\\", "\0");
            AudioClip audioClip;
            Sprite sprite;
            NoteFile noteFile;
            
            yield return FileManager.GetAudioClipRoutine(songDirPath + $"/{songTitle}.mp3", audioSource);
            audioClip = audioSource.clip;
            sprite = FileManager.GetSprite(songDirPath + $"/{songTitle}.png");
            noteFile = FileManager.GetNoteFile(songDirPath + $"/{songTitle}.note");

            Song song = new Song(noteFile, audioClip, sprite);
            loadedSongs.Add(song);
            
            GameObject newSong = Instantiate(songPrefab, songsHolder.transform);
            newSong.GetComponent<RectTransform>().localPosition = spawnPosition;
            spawnPosition.y -= 220f;
            newSong.name = songTitle;
            newSong.GetComponent<SongPanel>().SetSong(song);
        }

        public void PlaySongPreview(AudioClip audioClip)
        {
            if (audioSource.isPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(SongEndRoutine());
            }

            StartCoroutine(SongPreviewRoutine(audioClip));
        }

        private IEnumerator SongPreviewRoutine(AudioClip audioClip)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);

            yield return SongBeginRoutine(audioClip);

            yield return new WaitForSeconds(3f);

            yield return SongEndRoutine();
        }

        private IEnumerator SongBeginRoutine(AudioClip audioClip)
        {
            float currentTime = 0f;
            float percentage;

            audioSource.clip = audioClip;
            audioSource.time = audioClip.length / 2;
            audioSource.volume = startVolume;
            audioSource.Play();

            while (currentTime <= songEffectDuration)
            {
                currentTime += Time.deltaTime;

                percentage = currentTime / songEffectDuration;
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, percentage);

                yield return null;
            }
        }

        private IEnumerator SongEndRoutine()
        {
            float currentTime = 0f;
            float percentage;
            float songEffectDuration = this.songEffectDuration / 2;

            while (currentTime <= songEffectDuration)
            {
                currentTime += Time.deltaTime;

                percentage = currentTime / songEffectDuration;
                audioSource.volume = Mathf.Lerp(endVolume, startVolume, percentage);

                yield return null;
            }

            audioSource.Stop();
        }

        public int GetDifficulty()
        {
            return (int)difficulty;
        }

        public void UpdateDifficulty()
        {
            difficulty = (Difficulty)FileManager.GetUserSettings().difficulty;

            foreach (SongPanel songs in songsHolder.GetComponentsInChildren<SongPanel>())
            {
                Debug.Log(songs.name);
                //songs.GetComponent<SongPanel>().UpdateHighScore((int)difficulty);
            }
        }

        public void TryExportSong(string songTitle)
        {
            songToExport = songTitle;

            exportPrompt.SetActive(true);
        }

        // Function called by ExportConfirmationPrompt Click
        public void ResetExportSong()
        {
            songToExport = null;
        }

        // Function called by ExportConfirmationPrompt Click
        public void ExportSong()
        {
            if (songToExport == null)
                return;

            FileManager.ExportTapchFile(songToExport);
            Debug.Log($"I am exporting: {songToExport}");
        }

    }

}