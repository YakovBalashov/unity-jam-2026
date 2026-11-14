using UnityEngine;
using UnityEngine.SceneManagement;

namespace RacingGame.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            ShowMainPanel();
        }

        public void Play()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OpenSettings()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void CloseSettings()
        {
            ShowMainPanel();
        }

        private void ShowMainPanel()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }
    }
}