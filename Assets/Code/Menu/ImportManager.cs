using Saving;
using UnityEngine;


namespace Menu
{
    public class ImportManager : MonoBehaviour
    {
        [SerializeField]
        private Settings.SettingsManager settingsManager;

        private void Awake()
        {
            FileManager.CreateNecessaryStructure();    
        }

        public void ImportSong()
        {
            FileManager.ShowLoadDialog(ImportSongSucces, ImportSongCancel, "Load Song", FileManager.FileExtension.TAPCH);
        }

        private void ImportSongSucces(string[] paths)
        {
            string path = paths[0];

            if (path.Length == 0)
            {
                Debug.LogWarning("ImportTapchFile: File import fail");
                return;
            }

            if (!FileManager.ImportTapchFile(path))
            {
                Debug.LogWarning("ImportTapchFile: File import fail");
            }

            settingsManager.PlaySaveSound();
        }

        private void ImportSongCancel()
        {
            // TODO : Add error message
            settingsManager.PlayButtonSound();
        }
    }
}
