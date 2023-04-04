using Saving;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SongSelect
{
    public class SongPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text quePosition;
        [SerializeField]
        private Image image;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text author;
        [SerializeField] 
        private TMP_Text year;
        [SerializeField]
        private TMP_Text highScore;
        [SerializeField]
        private TMP_Text accuracy;
        private SongManager songManager;

        private Song song;

        private bool selected = false;

        private void Awake()
        {
            songManager = FindObjectOfType<SongManager>();
        }

        public void SetSong(Song song)
        {
            this.song = song;

            UpdateInfo();
        }
        
        private void UpdateInfo()
        {
            image.sprite = song.noteSprite;

            title.text = song.noteFile.title;
            author.text = song.noteFile.author;
            year.text = song.noteFile.year.ToString();

            UpdateScores(songManager.GetDifficulty());
        }

        public void UpdateScores(int difficulty)
        {
            Debug.Log($"{song.noteFile.title} diff changed");
            int score = song.noteFile.highScores[difficulty];
            float accuracyVal = song.noteFile.accuracies[difficulty];
            highScore.text = score.ToString();
            accuracy.text = $"{accuracyVal:0.##} %";
        }

        private void Enque()
        {
            SongManager.songsToPlay.Add(song);
            quePosition.text = SongManager.songsToPlay.Count.ToString();
            quePosition.gameObject.SetActive(true);
        }

        private void Deque()
        {
            SongManager.songsToPlay.Remove(song);
            quePosition.gameObject.SetActive(false);
        }

        public void PanelClick()
        {
            if (!selected) 
            {
                Enque();
                selected = true;
                songManager.PlaySongPreview(song.noteAudioClip);
            }
            else
            {
                Deque();
                selected = false;
            }
        }

        // Function called by ExportButton Click
        public void TryExportSong()
        {
            songManager.TryExportSong(song.noteFile.title);
        }

    }

}
