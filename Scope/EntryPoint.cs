#if UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public static class EntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void Run()
        {
            Application.targetFrameRate = 30;
#if UNITY_EDITOR
            var buildIndex = SceneManager.GetActiveScene().name;
            var scenes = new[] { "Boot", "MainMenu", "Game" };
            if (scenes.Contains(buildIndex)) await SceneManager.LoadSceneAsync("Boot");
            else return;
#endif
            Debug.Log("Startup");
            var gameObject = new GameObject(nameof(Bootstrap));
            var bootstrap = gameObject.AddComponent<Bootstrap>();
        }
    }
}