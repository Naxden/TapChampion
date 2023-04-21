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

        public Song Song { get; private set; }

        private bool selected = false;

        public void Initialize(Song song, SongManager songManager)
        {
            Song = song;
            this.songManager = songManager;
            UpdateInfo();
        }
        
        private void UpdateInfo()
        {
            image.sprite = Song.noteSprite;

            title.text = Song.noteFile.title;
            author.text = Song.noteFile.author;
            year.text = Song.noteFile.year.ToString();

            UpdateScores(songManager.GetDifficulty());
        }

        public void UpdateScores(int difficulty)
        {
            int score = Song.noteFile.highScores[difficulty];
            float accuracyVal = Song.noteFile.accuracies[difficulty];
            highScore.text = score.ToString();
            accuracy.text = $"{accuracyVal:0.##} %";
        }

        public void SetQueueNum(int queueNum)
        {
            quePosition.text = queueNum.ToString();
            quePosition.gameObject.SetActive(true);
        }

        private void Enque()
        {
            int queNum = songManager.EnqueSong(Song);
            SetQueueNum(queNum);
        }

        private void Deque()
        {
            songManager.DequeSong(Song);
            quePosition.gameObject.SetActive(false);
        }

        public void PanelClick()
        {
            if (!selected) 
            {
                Enque();
                selected = true;
                songManager.PlaySongPreview(Song.noteAudioClip);
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
            songManager.TryExportSong(Song.noteFile.title);
        }

    }

}
