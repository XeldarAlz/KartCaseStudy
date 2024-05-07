using Michsky.LSS;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Managers
{
    /// <summary>
    /// Manages scene transitions and loading screens using the Loading Screen Studio (LSS) system.
    /// </summary>
    [RequireComponent(typeof(LSS_Manager))]
    public class GameSceneManager : MonoBehaviour
    {
        [Tooltip("Prefab for the main menu loading screen located in Loading Screen Studio/Resources folder")]
        [SerializeField] private GameObject _mainMenuLoadingScreen;
        [Tooltip("Prefab for the in-game loading screen located in Loading Screen Studio/Resources folder")]
        [SerializeField] private GameObject _inGameLoadingScreen;

        [Inject] private readonly GameStateManager _gameStateManager;

        private const string DefaultLoadingPresetName = "Default";
        private string _mainMenuLoadingPresetName;
        private string _gameLoadingPresetName;
        private LSS_Manager _lssManager;

        private void Awake()
        {
            _lssManager = GetComponent<LSS_Manager>();
            _lssManager.SetSingle();
            _mainMenuLoadingPresetName = _mainMenuLoadingScreen ? _mainMenuLoadingScreen.name : DefaultLoadingPresetName;
            _gameLoadingPresetName = _inGameLoadingScreen ? _inGameLoadingScreen.name : DefaultLoadingPresetName;
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            LSS_LoadingScreen.LoadingStart += LoadingStart;
            LSS_LoadingScreen.LoadingEnd += LoadingEnd;
            LSS_LoadingScreen.LoadingDestroy += LoadingDestroy;
        }

        /// <summary>
        /// Loads the first scene at startup.
        /// </summary>
        public void LoadFirstSceneStartup()
        {
            _lssManager.SetPreset(DefaultLoadingPresetName);
            LoadScene(1);
        }

        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            _lssManager.SetPreset(_gameLoadingPresetName);
            LoadScene(1);
        }

        /// <summary>
        /// Loads the main game scene.
        /// </summary>
        public void LoadGameScene()
        {
            _lssManager.SetPreset(_mainMenuLoadingPresetName);
            LoadScene(2);
        }

        /// <summary>
        /// Loads a scene by its build index.
        /// </summary>
        /// <param name="sceneIndex">The build index of the scene to load.</param>
        public void LoadScene(int sceneIndex)
        {
            _lssManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
        }

        /// <summary>
        /// Handles the start of the loading process.
        /// </summary>
        private void LoadingStart()
        {
            _gameStateManager.LoadingStart();
        }

        /// <summary>
        /// Handles the end of the loading process.
        /// </summary>
        private void LoadingEnd()
        {
            _gameStateManager.LoadingEnd();
        }

        /// <summary>
        /// Handles the destruction of the loading screen.
        /// </summary>
        private void LoadingDestroy()
        {
            _gameStateManager.LoadingDestroy();
        }

        private void OnDisable()
        {
            LSS_LoadingScreen.LoadingStart -= LoadingStart;
            LSS_LoadingScreen.LoadingEnd -= LoadingEnd;
            LSS_LoadingScreen.LoadingDestroy -= LoadingDestroy;
        }
    }
}
