using Game.Events;

namespace Game.Managers
{
    /// <summary>
    /// Manages various types of events in the game.
    /// </summary>
    public class GameEventManager
    {
        private UiEvents _uiEvents;
        private SettingsEvents _settingsEvents;

        /// <summary>
        /// Gets the UI events.
        /// </summary>
        public UiEvents UiEvents => _uiEvents ??= new UiEvents();

        /// <summary>
        /// Gets the settings events.
        /// </summary>
        public SettingsEvents SettingsEvents => _settingsEvents ??= new SettingsEvents();
    }
}