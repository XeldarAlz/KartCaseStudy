using Game.States;

namespace Game.Player
{
    public class PlayerStateManager
    {
        public delegate void PlayerStateChangedDelegate(PlayerState oldState, PlayerState newState);
        public event PlayerStateChangedDelegate OnPlayerStateChanged;

        public delegate void PlayerBehaviourChangedDelegate(PlayerBehaviour oldBehaviour, PlayerBehaviour newBehaviour);
        public event PlayerBehaviourChangedDelegate OnPlayerBehaviourChanged;

        public delegate void PlayerStateSetDelegate();
        public event PlayerStateSetDelegate OnPlayerStateNone;
        public event PlayerStateSetDelegate OnPlayerStateIdle;
        public event PlayerStateSetDelegate OnPlayerStateWalking;
        public event PlayerStateSetDelegate OnPlayerStateRunning;
        public event PlayerStateSetDelegate OnPlayerStateJumping;
        public event PlayerStateSetDelegate OnPlayerStateLanded;
        public event PlayerStateSetDelegate OnPlayerStateSwimming;
        public event PlayerStateSetDelegate OnPlayerStatePaused;

        public delegate void PlayerBehaviourSetDelegate();
        public event PlayerBehaviourSetDelegate OnPlayerBehaviourNone;
        public event PlayerBehaviourSetDelegate OnPlayerBehaviourPaused;
        public event PlayerBehaviourSetDelegate OnPlayerBehaviourPlaying;
        public event PlayerBehaviourSetDelegate OnPlayerBehaviourCutScene;

        private PlayerState _playerState;
        private PlayerState _previousState;
        private PlayerBehaviour _playerBehaviour;

        public PlayerState GetState()
        {
            return _playerState;
        }
        public PlayerState GetPreviousState()
        {
            return _previousState;
        }
        public PlayerBehaviour GetBehaviour()
        {
            return _playerBehaviour;
        }

        public void SetPlayerState(PlayerState playerState)
        {
            PlayerState oldPlayerState = GetState();
            PlayerState newPlayerState = playerState;
            _previousState = oldPlayerState;

            _playerState = newPlayerState;
            PlayerStateChanged(oldPlayerState, newPlayerState);
        }
        public void SetPlayerBehaviour(PlayerBehaviour playerBehaviour)
        {
            PlayerBehaviour oldPlayerBehaviour = GetBehaviour();
            PlayerBehaviour newPlayerBehaviour = playerBehaviour;

            _playerBehaviour = newPlayerBehaviour;
            PlayerBehaviourChanged(oldPlayerBehaviour, newPlayerBehaviour);
        }
       
        public void PlayerStateChanged(PlayerState previousState, PlayerState newState)
        {
            OnPlayerStateChanged?.Invoke(previousState, newState);
        }
        public void PlayerBehaviourChanged(PlayerBehaviour previousBehaviour, PlayerBehaviour newBehaviour)
        {
            OnPlayerBehaviourChanged?.Invoke(previousBehaviour, newBehaviour);
        }

        public void PlayerStateSetNone()
        {
            OnPlayerStateNone?.Invoke();
            SetPlayerState(PlayerState.None);
        }
        public void PlayerStateSetIdle()
        {
            OnPlayerStateIdle?.Invoke();
            SetPlayerState(PlayerState.Idle);
        }
        public void PlayerStateSetWalking()
        {
            OnPlayerStateWalking?.Invoke();
            SetPlayerState(PlayerState.Walking);
        }
        public void PlayerStateSetRunning()
        {
            OnPlayerStateRunning?.Invoke();
            SetPlayerState(PlayerState.Running);
        }
        public void PlayerStateSetJumping()
        {
            OnPlayerStateJumping?.Invoke();
            SetPlayerState(PlayerState.Jumping);
        }
        public void PlayerStateSetLanded()
        {
            OnPlayerStateLanded?.Invoke();
            SetPlayerState(PlayerState.Landed);
        }
        public void PlayerStateSetSwimming()
        {
            OnPlayerStateSwimming?.Invoke();
            SetPlayerState(PlayerState.Swimming);
        }
        public void PlayerStateSetPaused()
        {
            OnPlayerStatePaused?.Invoke();
            SetPlayerState(PlayerState.Paused);
        }

        public void PlayerBehaviourSetNone()
        {
            OnPlayerBehaviourNone?.Invoke();
            SetPlayerBehaviour(PlayerBehaviour.None);
        }
        public void PlayerBehaviourSetPaused()
        {
            OnPlayerBehaviourPaused?.Invoke();
            SetPlayerBehaviour(PlayerBehaviour.Paused);
        }
        public void PlayerBehaviourSetPlaying()
        {
            OnPlayerBehaviourPlaying?.Invoke();
            SetPlayerBehaviour(PlayerBehaviour.Playing);
        }
        public void PlayerBehaviourSetCutScene()
        {
            OnPlayerBehaviourCutScene?.Invoke();
            SetPlayerBehaviour(PlayerBehaviour.CutScene);
        }
    }
}