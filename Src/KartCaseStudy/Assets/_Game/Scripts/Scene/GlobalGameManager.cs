using System;
using Game.Managers;
using Game.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.SceneManagers
{
    /// <summary>
    /// Manages global game behavior, including state transitions, scene management, and game flow.
    /// This class is responsible for responding to game state changes and managing the overall game lifecycle.
    /// </summary>
    public class GlobalGameManager : IInitializable, IDisposable
    {
        [Inject] private readonly GameSceneManager _gameSceneManager;
        [Inject] private readonly GameStateManager _gameStateManager;

        /// <summary>
        /// Initializes the game manager by subscribing to necessary game state events.
        /// This method is called when the game manager is first created.
        /// </summary>
        public void Initialize()
        {
            _gameStateManager.OnGameStateStarting += StartGame;
            _gameStateManager.OnGameStateExiting += ExitGame;
            _gameStateManager.OnGameStatePaused += PauseGame;
            _gameStateManager.OnGameStatePlaying += PlayGame;
            _gameStateManager.OnGameStateQuitting += QuitGame;
            _gameStateManager.OnLoadingDestroy += ResultLoading;

            DetermineInitialState();
        }

        /// <summary>
        /// Disposes the game manager by unsubscribing from game state events.
        /// This method is called when the game manager is being destroyed.
        /// </summary>
        public void Dispose()
        {
            _gameStateManager.OnGameStateStarting -= StartGame;
            _gameStateManager.OnGameStateExiting -= ExitGame;
            _gameStateManager.OnGameStatePaused -= PauseGame;
            _gameStateManager.OnGameStatePlaying -= PlayGame;
            _gameStateManager.OnGameStateQuitting -= QuitGame;
            _gameStateManager.OnLoadingDestroy -= ResultLoading;
        }

        /// <summary>
        /// Determines the initial state of the game based on the current scene index.
        /// </summary>
        private void DetermineInitialState()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            switch (index)
            {
                case 0:
                    LaunchGame();
                    break;
                case 1:
                    _gameStateManager.GameStateSetMainMenu();
                    break;
                default:
                    _gameStateManager.GameStateSetPlaying();
                    break;
            }
        }

        /// <summary>
        /// Launches the game by loading the first scene.
        /// </summary>
        private void LaunchGame()
        {
            _gameSceneManager.LoadFirstSceneStartup();
        }

        /// <summary>
        /// Starts the game by loading the game scene.
        /// </summary>
        private void StartGame()
        {
            _gameSceneManager.LoadGameScene();
        }

        /// <summary>
        /// Exits the current game and loads the main menu.
        /// </summary>
        private void ExitGame()
        {
            _gameSceneManager.LoadMainMenu();
        }

        /// <summary>
        /// Pauses the game, stopping time and showing the cursor.
        /// </summary>
        private void PauseGame()
        {
            Time.timeScale = 0.001f;
        }

        /// <summary>
        /// Resumes the game, starting time and hiding the cursor.
        /// </summary>
        private void PlayGame()
        {
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Handles the result of loading operations based on the current game state.
        /// </summary>
        private void ResultLoading()
        {
            switch (_gameStateManager.GetState())
            {
                case GameState.None:
                case GameState.Exiting:
                    _gameStateManager.GameStateSetMainMenu();
                    break;
                case GameState.Starting:
                    _gameStateManager.GameStateSetPlaying();
                    break;
            }
        }

        /// <summary>
        /// Quits the game application.
        /// </summary>
        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
