using Doozy.Runtime.UIManager.Containers;
using Game.Managers;
using UnityEngine;
using Zenject;

namespace Game.UserInterface.InGame
{
    /// <summary>
    /// Controls the in-game UI, managing the display of different views based on the game state.
    /// Responds to game state changes and user inputs to control the visibility of different UI views.
    /// </summary>
    public class InGameUIController : MonoBehaviour
    {
        [Header("View Controllers")]
        [SerializeField] private UIView _playerView;
        [SerializeField] private UIView _pauseView;
        [SerializeField] private UIView _settingsView;

        [Inject] private readonly GameEventManager _gameEventManager;
        [Inject] private readonly GameStateManager _gameStateManager;

        private void OnEnable()
        {
            _gameStateManager.OnGameStatePlaying += EnablePlayerView;
            _gameStateManager.OnGameStatePaused += EnablePauseView;
            _gameStateManager.OnGameStateSettings += EnableSettingsView;
        }

        private void OnDisable()
        {
            _gameStateManager.OnGameStatePlaying -= EnablePlayerView;
            _gameStateManager.OnGameStatePaused -= EnablePauseView;
            _gameStateManager.OnGameStateSettings -= EnableSettingsView;
        }

        #region Button References

        /// <summary>
        /// Handles the action when the resume button is pressed.
        /// Triggers the event to resume the game.
        /// </summary>
        public void ResumeButtonPressed()
        {
            _gameEventManager.UiEvents.ResumeButtonPressed();
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
        /// Handles the action when the exit button is pressed.
        /// Triggers the event to exit the game.
        /// </summary>
        public void ExitButtonPressed()
        {
            _gameEventManager.UiEvents.ExitGameButtonPressed();
        }

        /// <summary>
        /// Handles the action when the pause button is pressed.
        /// Triggers the event to pause the game.
        /// </summary>
        public void PauseButtonPressed()
        {
            _gameStateManager.GameStateSetPaused();
        }

        #endregion

        /// <summary>
        /// Enables the player view and hides other views.
        /// Called when the game state changes to playing.
        /// </summary>
        private void EnablePlayerView()
        {
            _settingsView.Hide();
            _pauseView.Hide();
            _playerView.Show();
        }

        /// <summary>
        /// Enables the pause view and hides other views.
        /// Called when the game state changes to paused.
        /// </summary>
        private void EnablePauseView()
        {
            _settingsView.Hide();
            _playerView.Hide();
            _pauseView.Show();
        }

        /// <summary>
        /// Enables the settings view and hides other views.
        /// Called when the game state changes to settings.
        /// </summary>
        private void EnableSettingsView()
        {
            _pauseView.Hide();
            _playerView.Hide();
            _settingsView.Show();
        }
    }
}