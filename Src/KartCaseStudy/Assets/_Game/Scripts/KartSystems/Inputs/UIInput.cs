using UnityEngine;
using UnityEngine.EventSystems;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace KartSystem.KartSystems
{
    /// <summary>
    /// <para>A class that implements the IInput interface and provides basic input functionality for a kart using the Unity Input System.</para>
    /// <para>This class provides properties for acceleration, steering, hop button press, and hop button hold states. It also handles touch events for joystick control.</para>
    /// </summary>
    public class UIInput : MonoBehaviour, IInput
    {
        /// <summary>
        /// The size of the joystick in pixels.
        /// </summary>
        public float Acceleration { get; private set; }

        /// <summary>
        ///     Gets the steering value.
        /// </summary>
        public float Steering { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the hop button is pressed.
        /// </summary>
        public bool HopPressed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the hop button is held down.
        /// </summary>
        public bool HopHeld { get; set; }

        [Tooltip("The size of the joystick.")]
        [SerializeField] private Vector2 JoystickSize = new(300, 300);

        [Tooltip("The joystick component.")]
        [SerializeField] private FloatingJoystick Joystick;

        private ETouch.Finger _movementFinger;
        private Vector2 _movementAmount;
        private bool _fixedUpdateHappened;

        /// <summary>
        ///     Called when the pointer is pressed down. Sets HopPressed and HopHeld to true.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            HopPressed = true;
            HopHeld = true;
        }

        /// <summary>
        ///     Called when the pointer is released. Sets HopHeld and HopPressed to false.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            HopHeld = false;
            HopPressed = false;
        }

        private void OnEnable()
        {
            ETouch.EnhancedTouchSupport.Enable();
            ETouch.Touch.onFingerDown += HandleFingerDown;
            ETouch.Touch.onFingerUp += HandleLoseFinger;
            ETouch.Touch.onFingerMove += HandleFingerMove;
        }


        private void OnDisable()
        {
            ETouch.Touch.onFingerDown -= HandleFingerDown;
            ETouch.Touch.onFingerUp -= HandleLoseFinger;
            ETouch.Touch.onFingerMove -= HandleFingerMove;
            ETouch.EnhancedTouchSupport.Disable();
        }

        /// <summary>
        /// <para>This function is called when a finger moves on the screen and updates the joystick knob position and movement amount.</para>
        /// </summary>
        /// <param name="movedFinger">The finger that moved on the screen.</param>
        private void HandleFingerMove(ETouch.Finger movedFinger)
        {
            if (movedFinger == _movementFinger)
            {
                Vector2 knobPosition;
                var maxMovement = JoystickSize.x / 2f;
                var currentTouch = movedFinger.currentTouch;

                if (Vector2.Distance(
                        currentTouch.screenPosition,
                        Joystick.RectTransform.anchoredPosition
                    ) > maxMovement)
                    knobPosition = (
                                       currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                                   ).normalized
                                   * maxMovement;
                else
                    knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;

                Joystick.Knob.anchoredPosition = knobPosition;
                _movementAmount = knobPosition / maxMovement;
            }
        }

        /// <summary>
        /// <para>This function is called when a finger is lifted from the screen and resets the joystick and movement amount.</para>
        /// </summary>
        /// <param name="lostFinger">The finger that was lifted from the screen.</param>
        private void HandleLoseFinger(ETouch.Finger lostFinger)
        {
            if (lostFinger == _movementFinger)
            {
                _movementFinger = null;
                Joystick.Knob.anchoredPosition = Vector2.zero;
                Joystick.gameObject.SetActive(false);
                _movementAmount = Vector2.zero;
            }
        }

        /// <summary>
        ///     Handles finger press. Activates the joystick and sets the movement finger.
        /// </summary>
        private void HandleFingerDown(ETouch.Finger touchedFinger)
        {
            if (_movementFinger == null && touchedFinger.screenPosition.x <= Screen.width / 2f)
            {
                _movementFinger = touchedFinger;
                _movementAmount = Vector2.zero;
                Joystick.gameObject.SetActive(true);
                Joystick.RectTransform.sizeDelta = JoystickSize;
                Joystick.RectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
            }
        }

        /// <summary>
        /// <para>This function clamps the start position of the joystick within the screen bounds.</para>
        /// </summary>
        /// <param name="startPosition">The start position of the joystick.</param>
        /// <returns>The clamped start position.</returns>
        private Vector2 ClampStartPosition(Vector2 startPosition)
        {
            if (startPosition.x < JoystickSize.x / 2) startPosition.x = JoystickSize.x / 2;

            if (startPosition.y < JoystickSize.y / 2)
                startPosition.y = JoystickSize.y / 2;
            else if (startPosition.y > Screen.height - JoystickSize.y / 2)
                startPosition.y = Screen.height - JoystickSize.y / 2;

            return startPosition;
        }

        private void Update()
        {
            Acceleration = _movementAmount.y;
            Steering = _movementAmount.x;
        }
    }
}