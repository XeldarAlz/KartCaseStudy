using Game.States;

namespace Game.Managers
{
    /// <summary>
    /// Manages the various states of the game and triggers events on state changes.
    /// </summary>
    public class GameStateManager
    {
        /// <summary>
        /// Delegate for handling game state changes.
        /// </summary>
        /// <param name="oldState">The previous game state.</param>
        /// <param name="newState">The new game state.</param>
        public delegate void GameStateChangedDelegate(GameState oldState, GameState newState);

        /// <summary>
        /// Triggered when the game state changes.
        /// </summary>
        public event GameStateChangedDelegate OnGameStateChanged;

        /// <summary>
        /// Delegate for handling specific game state settings.
        /// </summary>
        public delegate void GameStateSetDelegate();

        // Events for each specific game state
        public event GameStateSetDelegate OnGameStateNone;
        public event GameStateSetDelegate OnGameStateMainMenu;
        public event GameStateSetDelegate OnGameStateStarting;
        public event GameStateSetDelegate OnGameStateSettings;
        public event GameStateSetDelegate OnGameStateLoading;
        public event GameStateSetDelegate OnGameStateSaving;
        public event GameStateSetDelegate OnGameStatePlaying;
        public event GameStateSetDelegate OnGameStateResuming;
        public event GameStateSetDelegate OnGameStatePaused;
        public event GameStateSetDelegate OnGameStateDead;
        public event GameStateSetDelegate OnGameStateIdle;
        public event GameStateSetDelegate OnGameStateReviving;
        public event GameStateSetDelegate OnGameStateExiting;
        public event GameStateSetDelegate OnGameStateQuitting;

        /// <summary>
        /// Delegate for handling game loading events.
        /// </summary>
        public delegate void GameLoadingDelegate();

        // Events for game loading
        public event GameLoadingDelegate OnLoadingStart;
        public event GameLoadingDelegate OnLoadingEnd;
        public event GameLoadingDelegate OnLoadingDestroy;

        private GameState _gameState;

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        /// <returns>The current game state.</returns>
        public GameState GetState()
        {
            return _gameState;
        }

        /// <summary>
        /// Sets the game state and triggers the state changed event.
        /// </summary>
        /// <param name="gameState">The new game state to set.</param>
        public void SetState(GameState gameState)
        {
            if (_gameState != gameState)
            {
                GameState oldGameState = _gameState;
                _gameState = gameState;
                OnGameStateChanged?.Invoke(oldGameState, gameState);
            }
        }

        /// <summary>
        /// Sets the game state to a specific state and triggers the corresponding event.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        /// <param name="stateEvent">The event to trigger.</param>
        private void ChangeState(GameState newState, GameStateSetDelegate stateEvent)
        {
            stateEvent?.Invoke();
            SetState(newState);
        }

        // Methods for setting each specific game state
        public void GameStateSetNone() => ChangeState(GameState.None, OnGameStateNone);
        public void GameStateSetMainMenu() => ChangeState(GameState.MainMenu, OnGameStateMainMenu);
        public void GameStateSetStarting() => ChangeState(GameState.Starting, OnGameStateStarting);
        public void GameStateSetSettings() => ChangeState(GameState.Settings, OnGameStateSettings);
        public void GameStateSetLoading() => ChangeState(GameState.Loading, OnGameStateLoading);
        public void GameStateSetSaving() => ChangeState(GameState.Saving, OnGameStateSaving);
        public void GameStateSetPlaying() => ChangeState(GameState.Playing, OnGameStatePlaying);
        public void GameStateSetResuming() => ChangeState(GameState.Resuming, OnGameStateResuming);
        public void GameStateSetPaused() => ChangeState(GameState.Paused, OnGameStatePaused);
        public void GameStateSetDead() => ChangeState(GameState.Dead, OnGameStateDead);
        public void GameStateSetIdle() => ChangeState(GameState.Idle, OnGameStateIdle);
        public void GameStateSetReviving() => ChangeState(GameState.Reviving, OnGameStateReviving);
        public void GameStateSetExiting() => ChangeState(GameState.Exiting, OnGameStateExiting);
        public void GameStateSetQuitting() => ChangeState(GameState.Quitting, OnGameStateQuitting);

        // Methods for handling loading events
        public void LoadingStart() => OnLoadingStart?.Invoke();
        public void LoadingEnd() => OnLoadingEnd?.Invoke();
        public void LoadingDestroy() => OnLoadingDestroy?.Invoke();
    }
}
