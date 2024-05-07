using Game.Events;

namespace Game.Managers
{
    /// <summary>
    /// Manages various types of events in the game.
    /// </summary>
    public class GameEventManager
    {
        private GameEvents _gameEvents;
        private UiEvents _uiEvents;
        private PlayerEvents _playerEvents;
        private SettingsEvents _settingsEvents;

        /// <summary>
        /// Gets the game events.
        /// </summary>
        public GameEvents GameEvents => _gameEvents ??= new GameEvents();

        /// <summary>
        /// Gets the UI events.
        /// </summary>
        public UiEvents UiEvents => _uiEvents ??= new UiEvents();

        /// <summary>
        /// Gets the player events.
        /// </summary>
        public PlayerEvents PlayerEvents => _playerEvents ??= new PlayerEvents();

        /// <summary>
        /// Gets the settings events.
        /// </summary>
        public SettingsEvents SettingsEvents => _settingsEvents ??= new SettingsEvents();
    }
}