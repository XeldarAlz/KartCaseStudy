using Game.Managers;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    /// <summary>
    /// Controls the player's movement.
    /// It handles input for movement and applies gravity and other physics-related effects.
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        // [Inject] private readonly PlayerMovementSettings _playerMovementSettings;
        [Inject] private readonly GameInputManager _gameInputManager;
        [Inject] private readonly GameStateManager _gameStateManager;
        [Inject] private readonly WorldManager _worldManager;

        //Private Fields


        private void OnEnable()
        {
            // _gameStateManager.OnGameStatePlaying += OnGameStatePlaying;
            // _gameStateManager.OnGameStatePaused += OnGameStatePaused;

            // _gameInputManager.OnMoveInput += OnMoveInput;
        }

        private void OnDisable()
        {
            // _gameStateManager.OnGameStatePlaying -= OnGameStatePlaying;
            // _gameStateManager.OnGameStatePaused -= OnGameStatePaused;

            // _gameInputManager.OnMoveInput -= OnMoveInput;

        }

        /// <summary>
        /// Handles player movement updates, including switching between underwater and ground movement,
        /// and applying gravity when necessary.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Used to apply consistent gravity effects to the player.
        /// </summary>
        private void FixedUpdate()
        {

        }

        /// <summary>
        /// Processes movement input from the player.
        /// Captures the movement direction based on player input.
        /// </summary>
        /// <param name="input">The movement input vector, representing the direction of movement.</param>
        // private void OnMoveInput(Vector2 input)
        // {
        //     _moveInput = input;
        // }

        /// <summary>
        /// Handles the rotation of the player based on look input.
        /// Adjusts the player's orientation in the game world to match the direction they are looking.
        /// </summary>
        // private void HandleRotation()
        // {

        // }

        /// <summary>
        /// Handles the player's movement on the ground.
        /// Calculates and applies movement based on player input, considering current move speed.
        /// </summary>
        // private void HandleGroundMovement()
        // {

        // }

        /// <summary>
        /// Checks if the player is grounded.
        /// Performs a raycast to determine if the player is standing on the ground.
        /// </summary>
        /// <returns>True if the player is grounded, false otherwise.</returns>
        // private bool IsGrounded()
        // {
        //     Vector3 rayStart = transform.position + _characterController.center;
        //     float rayLength = _characterController.center.y + 0.01f + _playerMovementSettings.GroundCheckDistance;
        //     bool hasHit = Physics.Raycast(rayStart, Vector3.down, out _, rayLength);
        //     return hasHit;
        // }

        /// <summary>
        /// Applies gravity to the player.
        /// Consistently applies downward force to simulate gravity, affecting the player's vertical movement.
        /// </summary>
        // private void ApplyGravity()
        // {
        //     _gravityMovement = new Vector3(0, _verticalVelocity, 0);
        //     _characterController.Move(_gravityMovement * Time.deltaTime);
        // }

        /// <summary>
        /// Handles actions to be performed when the game state changes to playing.
        /// Enables player movement capabilities.
        /// </summary>
        // private void OnGameStatePlaying()
        // {
        //     _playerMovementSettings.CanMove = true;
        // }

        /// <summary>
        /// Handles actions to be performed when the game state changes to paused.
        /// Disables player movement capabilities.
        /// </summary>
        // private void OnGameStatePaused()
        // {
        //     _playerMovementSettings.CanMove = false;
        // }
    }
}