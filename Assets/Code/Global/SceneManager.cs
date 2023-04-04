using System.Collections;
using UnityEngine;

namespace Global
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField]
        private FadeManger fadeManager;

        private void Start()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
                StartCoroutine(fadeManager.FadeRoutine(false));
        }

        public string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            yield return fadeManager.FadeRoutine(true);

            if (sceneName == "Exit")
                Application.Quit();

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}

