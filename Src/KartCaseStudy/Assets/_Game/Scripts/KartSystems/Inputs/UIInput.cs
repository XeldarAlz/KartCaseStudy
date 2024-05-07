using UnityEngine;
using UnityEngine.EventSystems;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace KartGame.KartSystems
{
    /// <summary>
    ///     A basic keyboard implementation of the IInput interface for all the input information a kart needs.
    /// </summary>
    public class UIInput : MonoBehaviour, IInput
    {
        public float Acceleration { get; private set; }

        public float Steering { get; private set; }

        public bool HopPressed
        {
            get => m_HopPressed;
            set => m_HopPressed = value;
        }

        public bool HopHeld => m_HopHeld;

        [SerializeField] private readonly Vector2 JoystickSize = new(300, 300);
        [SerializeField] private FloatingJoystick Joystick;

        private ETouch.Finger MovementFinger;
        private Vector2 MovementAmount;
        public bool m_HopPressed;
        public bool m_HopHeld;
        private bool m_FixedUpdateHappened;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_HopPressed = true;
            m_HopHeld = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_HopHeld = false;
            m_HopPressed = false;
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

        private void HandleFingerMove(ETouch.Finger MovedFinger)
        {
            if (MovedFinger == MovementFinger)
            {
                Vector2 knobPosition;
                var maxMovement = JoystickSize.x / 2f;
                var currentTouch = MovedFinger.currentTouch;

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
                MovementAmount = knobPosition / maxMovement;
            }
        }

        private void HandleLoseFinger(ETouch.Finger LostFinger)
        {
            if (LostFinger == MovementFinger)
            {
                MovementFinger = null;
                Joystick.Knob.anchoredPosition = Vector2.zero;
                Joystick.gameObject.SetActive(false);
                MovementAmount = Vector2.zero;
            }
        }

        private void HandleFingerDown(ETouch.Finger TouchedFinger)
        {
            if (MovementFinger == null && TouchedFinger.screenPosition.x <= Screen.width / 2f)
            {
                MovementFinger = TouchedFinger;
                MovementAmount = Vector2.zero;
                Joystick.gameObject.SetActive(true);
                Joystick.RectTransform.sizeDelta = JoystickSize;
                Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
            }
        }

        private Vector2 ClampStartPosition(Vector2 StartPosition)
        {
            if (StartPosition.x < JoystickSize.x / 2) StartPosition.x = JoystickSize.x / 2;

            if (StartPosition.y < JoystickSize.y / 2)
                StartPosition.y = JoystickSize.y / 2;
            else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
                StartPosition.y = Screen.height - JoystickSize.y / 2;

            return StartPosition;
        }

        public void SetHopPressed(bool value)
        {
            m_HopPressed = value;
        }

        public void SetHopHeld(bool value)
        {
            m_HopHeld = value;
        }


        private void Update()
        {
            Acceleration = MovementAmount.y;
            Steering = MovementAmount.x;

            // m_HopHeld = Keyboard.current.spaceKey.isPressed;

            // if (m_FixedUpdateHappened)
            // {
            //     m_FixedUpdateHappened = false;
            //     m_HopPressed = false;
            // }

            // m_HopPressed |= Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        // private void FixedUpdate()
        // {
        //     m_FixedUpdateHappened = true;
        // }
    }
}