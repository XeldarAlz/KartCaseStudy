using KartSystem.Timeline;
using UnityEngine;

namespace KartSystem.UI
{
    /// <summary>
    /// The MetaGameController is responsible for switching control between the high level
    /// contexts of the application, eg the Main Menu and Gameplay systems.
    /// </summary>
    public class MetaGameController : MonoBehaviour
    {
        [Tooltip("A reference to the main menu.")]
        public MainUIController mainMenu;
        [Tooltip("A reference to the race countdown director trigger.")]
        public DirectorTrigger raceCountdownTrigger;

        private bool _firstTime = true;

        private void Start()
        {
            HandleMenuButton();
        }


        private void HandleMenuButton()
        {
            if (_firstTime)
            {
                raceCountdownTrigger.TriggerDirector();
                _firstTime = false;
            }
        }
    }
}
