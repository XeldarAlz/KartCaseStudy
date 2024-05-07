using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;


namespace Game.Managers
{
    /// <summary>
    /// Manages game input, translating Unity Input System actions into game events.
    /// </summary>
    public class GameInputManager : MonoBehaviour
    {
        /// <summary>
        /// Delegate for handling vector-based input events.
        /// </summary>
        /// <param name="input">The input vector.</param>
        public delegate void VectorInputDelegate(Vector2 input);

        /// <summary>
        /// Delegate for handling button-based input events.
        /// </summary>
        public delegate void ButtonInputDelegate();

        public delegate void FingerInputDelegate(Finger finger);

        public event VectorInputDelegate OnMoveInput;
        public event ButtonInputDelegate OnBack;
        public event ButtonInputDelegate OnJumpStarted;
        public event ButtonInputDelegate OnJumpPerformed;
        public event ButtonInputDelegate OnJumpCanceled;

        public event FingerInputDelegate OnFingerDown;
        public event FingerInputDelegate OnFingerUp;
        public event FingerInputDelegate OnFingerMove;

        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput != null)
            {
                RegisterInputActions();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("<color=red>PlayerInput component not found on the GameObject.</color>", this);
#endif
            }
        }

        private void OnDestroy()
        {
            UnregisterInputActions();
        }

        /// <summary>
        /// Registers input actions to their respective events.
        /// </summary>
        private void RegisterInputActions()
        {
            if (_playerInput.currentActionMap == null)
            {
#if UNITY_EDITOR
                Debug.LogError("<color=red>Current action map is null.</color>");
#endif
                return;
            }

            // Enable inputs.
            _playerInput.currentActionMap.Enable();
            EnhancedTouchSupport.Enable();

            // Registering all the input actions.
            _playerInput.currentActionMap.FindAction("Move").performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
            _playerInput.currentActionMap.FindAction("Move").canceled += ctx => OnMoveInput?.Invoke(Vector2.zero);
            _playerInput.currentActionMap.FindAction("Back").performed += ctx => OnBack?.Invoke();
            _playerInput.currentActionMap.FindAction("Jump").started += ctx => OnJumpStarted?.Invoke();
            _playerInput.currentActionMap.FindAction("Jump").performed += ctx => OnJumpCanceled?.Invoke();
            _playerInput.currentActionMap.FindAction("Jump").canceled += ctx => OnJumpCanceled?.Invoke();


            // Registering finger input actions.
            ETouch.Touch.onFingerDown += fgr => OnFingerDown?.Invoke(fgr);
            ETouch.Touch.onFingerUp += fgr => OnFingerUp?.Invoke(fgr);
            ETouch.Touch.onFingerMove += fgr => OnFingerDown?.Invoke(fgr);
        }

        /// <summary>
        /// Unregisters input actions and cleans up event bindings.
        /// </summary>
        private void UnregisterInputActions()
        {
            if (_playerInput.currentActionMap == null) return;

            _playerInput.currentActionMap.Disable();
            EnhancedTouchSupport.Disable();
        }
    }
}
