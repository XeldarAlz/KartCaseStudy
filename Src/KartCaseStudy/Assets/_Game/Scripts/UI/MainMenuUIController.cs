using Doozy.Runtime.UIManager.Containers;
using Febucci.UI;
using Game.Managers;
using UnityEngine;
using Zenject;

namespace Game.UserInterface.MainMenu
{
    /// <summary>
    /// Manages the main menu UI, handling the display and transitions between the main menu and settings views.
    /// Responds to game state changes and user inputs to control the visibility of different UI views.
    /// </summary>
    public class MainMenuUIController : MonoBehaviour
    {
        [Header("View Controllers")] 
        [SerializeField] private UIView _mainMenuView;
        [SerializeField] private UIView _settingsView;

        [Header("Graphics Settings")] 
        [SerializeField] private string _graphicsSettingsAppliedFeedback;
        [SerializeField] private TypewriterByCharacter _feedbackText;

        [Inject] private readonly GameEventManager _gameEventManager;
        [Inject] private readonly GameStateManager _gameStateManager;
        [Inject] private readonly GameInputManager _gameInputManager;

        private void OnEnable()
        {
            _gameStateManager.OnGameStateMainMenu += EnableMainMenu;
            _gameStateManager.OnGameStateSettings += EnableSettingsMenu;
        }

        private void OnDisable()
        {
            _gameStateManager.OnGameStateMainMenu -= EnableMainMenu;
            _gameStateManager.OnGameStateSettings -= EnableSettingsMenu;
        }

        #region Button References

        /// <summary>
        /// Handles the action when the start game button is pressed.
        /// Triggers the event to start the game.
        /// </summary>
        public void StartGameButtonPressed()
        {
            _gameEventManager.UiEvents.StartGameButtonPressed();
        }

        /// <summary>
        /// Handles the action when the settings button is pressed.
        /// Triggers the event to open the settings view.
        /// </summary>
        public void SettingsButtonPressed()
        {
            _gameEventManager.UiEvents.SettingsButtonPressed();
        }

        /// <summary>
        /// Handles the action when the quit game button is pressed.
        /// Triggers the event to quit the game.
        /// </summary>
        public void QuitGameButtonPressed()
        {
            _gameEventManager.UiEvents.QuitGameButtonPressed();
        }

        #endregion

        /// <summary>
        /// Enables the main menu view and hides the settings view.
        /// Called when the game state changes to the main menu.
        /// </summary>
        public void EnableMainMenu()
        {
            _mainMenuView.Show();
            _settingsView.Hide();
        }

        /// <summary>
        /// Enables the settings menu view and hides the main menu view.
        /// Called when the game state changes to settings.
        /// </summary>
        public void EnableSettingsMenu()
        {
            _mainMenuView.Hide();
            _settingsView.Show();
        }
    }
}