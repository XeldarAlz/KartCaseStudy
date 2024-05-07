using Game.Managers;
using Game.States;
using UnityEngine;
using Zenject;

namespace Game.SceneManagers
{
    /// <summary>
    /// Manages in-game scene state transitions based on UI events and player input.
    /// This class listens to various UI and input events to control the game's state,
    /// such as pausing, resuming, and exiting the game.
    /// </summary>
    public class InGameSceneStateManager : MonoBehaviour
    {
        [Inject] private readonly GameStateManager _gameStateManager;
        [Inject] private readonly GameEventManager _gameEventManager;
        [Inject] private readonly GameInputManager _gameInputManager;

        private void OnEnable()
        {
            _gameEventManager.UiEvents.OnResumeButtonPressed += OnResumeButtonPressed;
            _gameEventManager.UiEvents.OnSettingsButtonPressed += OnSettingsButtonPressed;
            _gameEventManager.UiEvents.OnExitGameButtonPressed += OnExitButtonPressed;
            _gameEventManager.UiEvents.OnBackButtonPressed += OnBackButtonPressed;
            _gameInputManager.OnBack += OnBackPressed;
        }

        private void OnDisable()
        {
            _gameEventManager.UiEvents.OnResumeButtonPressed -= OnResumeButtonPressed;
            _gameEventManager.UiEvents.OnSettingsButtonPressed -= OnSettingsButtonPressed;
            _gameEventManager.UiEvents.OnExitGameButtonPressed -= OnExitButtonPressed;
            _gameEventManager.UiEvents.OnBackButtonPressed -= OnBackButtonPressed;
            _gameInputManager.OnBack -= OnBackPressed;
        }

        private void OnResumeButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.Paused)
            {
                _gameStateManager.GameStateSetPlaying();
            }
        }

        private void OnSettingsButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.Paused)
            {
                _gameStateManager.GameStateSetSettings();
            }
        }

        private void OnExitButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.Paused)
            {
                _gameStateManager.GameStateSetExiting();
            }
        }

        private void OnBackButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.Settings)
            {
                _gameStateManager.GameStateSetPaused();
            }
        }

        private void OnBackPressed()
        {
            switch (_gameStateManager.GetState())
            {
                case GameState.Playing:
                case GameState.Settings:
                {
                    _gameStateManager.GameStateSetPaused();
                    break;
                }

                case GameState.Paused:
                {
                    _gameStateManager.GameStateSetPlaying();
                        break;
                }
            }
        }
    }
}