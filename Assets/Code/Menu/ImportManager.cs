using Saving;
using UnityEngine;


namespace Menu
{
    public class ImportManager : MonoBehaviour
    {

        public void ImportSong()
        {
            FileManager.ShowLoadDialog(ImportSongSucces, ImportSongFail, "Load Song", FileManager.FileExtension.TAPCH);
        }
        private void ImportSongSucces(string[] paths)
        {
            string path = paths[0];

            if (path.Length == 0)
            {
                Debug.Log("ImportTapchFile: File import fail");
                return;
            }

            if (!FileManager.ImportTapchFile(path))
            {
                Debug.Log("ImportTapchFile: File import fail");
            }
        }

        private void ImportSongFail()
        {

        }
    }
}
