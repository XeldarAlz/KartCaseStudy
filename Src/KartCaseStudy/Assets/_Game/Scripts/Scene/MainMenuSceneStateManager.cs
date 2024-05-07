using Game.Managers;
using Game.States;
using UnityEngine;
using Zenject;

namespace Game.SceneManagers
{
    /// <summary>
    /// Manages state transitions in the main menu scene based on UI events and player input.
    /// This class listens to various UI and input events to control transitions between different states
    /// like starting the game, opening settings, or quitting the game from the main menu.
    /// </summary>
    public class MainMenuSceneStateManager : MonoBehaviour
    {
        [Inject] private readonly GameEventManager _gameEventManager;
        [Inject] private readonly GameStateManager _gameStateManager;
        [Inject] private readonly GameInputManager _gameInputManager;

        private void OnEnable()
        {
            _gameEventManager.UiEvents.OnStartButtonPressed += OnStartButtonPressed;
            _gameEventManager.UiEvents.OnSettingsButtonPressed += OnSettingsButtonPressed;
            _gameEventManager.UiEvents.OnQuitGameButtonPressed += OnQuitGameButtonPressed;
            _gameEventManager.UiEvents.OnBackButtonPressed += OnBackButtonPressed;
            _gameInputManager.OnBack += OnBackKeyInput;
        }

        private void OnDisable()
        {
            _gameEventManager.UiEvents.OnStartButtonPressed -= OnStartButtonPressed;
            _gameEventManager.UiEvents.OnSettingsButtonPressed -= OnSettingsButtonPressed;
            _gameEventManager.UiEvents.OnExitGameButtonPressed -= OnQuitGameButtonPressed;
            _gameEventManager.UiEvents.OnBackButtonPressed -= OnBackButtonPressed;
            _gameInputManager.OnBack -= OnBackKeyInput;
        }

        private void OnStartButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.MainMenu)
            {
                _gameStateManager.GameStateSetStarting();
            }
        }

        private void OnSettingsButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.MainMenu)
            {
                _gameStateManager.GameStateSetSettings();
            }
        }

        private void OnQuitGameButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.MainMenu)
            {
                _gameStateManager.GameStateSetQuitting();
            }
        }

        private void OnBackButtonPressed()
        {
            if (_gameStateManager.GetState() == GameState.Settings)
            {
                _gameStateManager.GameStateSetMainMenu();
            }
        }

        private void OnBackKeyInput()
        {
            if (_gameStateManager.GetState() == GameState.Settings)
            {
                _gameStateManager.GameStateSetMainMenu();
            }
        }
    }
}