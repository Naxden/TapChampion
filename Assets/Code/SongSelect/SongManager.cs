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

        private Vector3 spawnPosition = new Vector3(0f, 200f, 0f);
        private List<Song> loadedSongs = new List<Song>();
        public static List<Song> songsToPlay = new List<Song>();

        private const float startVolume = 0.1f;
        float endVolume = 1.0f;
        [SerializeField]
        float songEffectDuration = 0.8f;

        private void Start()
        {
            if (songsToPlay.Count > 0)
                songsToPlay.Clear();

            string[] paths = FileManager.GetAllSongs();

            foreach (string path in paths)
            {
                InstantiateSong(path);
            }
        }

        public void InstantiateSong(string songDirPath)
        {
            StartCoroutine(InstantiateSongRoutine(songDirPath));
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
            Debug.Log("Starting Begin Routine");
            
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
            Debug.Log("Ending Begin Routine");
        }

        public void PauseSongPreview()
        {
            return;
            StopAllCoroutines();
            
            if (audioSource.isPlaying)
                StartCoroutine(SongEndRoutine());
        }

        private IEnumerator SongEndRoutine()
        {
            Debug.Log("Starting End Routine");

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
            Debug.Log("Ending End Routine");
        }
    }

}