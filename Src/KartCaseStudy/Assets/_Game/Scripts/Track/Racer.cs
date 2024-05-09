using System.Collections.Generic;
using KartSystem.KartSystems;
using UnityEngine;

namespace KartSystem.Track
{
    /// <summary>
    ///     The default implementation of the IRacer interface.  This is a representation of all the timing information for a
    ///     particular kart as it goes through a race.
    /// </summary>
    public class Racer : MonoBehaviour, IRacer
    {
        [Tooltip("A reference to the IControllable for the kart.  Normally this is the KartMovement script.")]
        [RequireInterface(typeof(IControllable))]
        public Object kartMovement;

        private IControllable _kartMovement;
        private bool _isTimerPaused = true;
        public float _timer;
        public int _currentLap;
        public List<float> _lapTimes = new(9);

        private void Awake()
        {
            _kartMovement = kartMovement as IControllable;
        }

        private void Update()
        {
            if (_currentLap > 0 && !_isTimerPaused) _timer += Time.deltaTime;
        }

        public void PauseTimer()
        {
            _isTimerPaused = true;
        }

        public void UnpauseTimer()
        {
            _isTimerPaused = false;
        }

        public void HitStartFinishLine()
        {
            if (_currentLap > 0)
            {
                _lapTimes.Add(_timer);
                _timer = 0f;
            }

            _currentLap++;
        }

        public int GetCurrentLap()
        {
            return _currentLap;
        }

        public List<float> GetLapTimes()
        {
            return _lapTimes;
        }

        public float GetLapTime()
        {
            return _timer;
        }

        public float GetRaceTime()
        {
            var raceTime = _timer;
            for (var i = 0; i < _lapTimes.Count; i++) raceTime += _lapTimes[i];

            return raceTime;
        }

        public void EnableControl()
        {
            _kartMovement.EnableControl();
        }

        public void DisableControl()
        {
            _kartMovement.DisableControl();
        }

        public bool IsControlled()
        {
            return _kartMovement.IsControlled();
        }

        public string GetName()
        {
            return name;
        }
    }
}