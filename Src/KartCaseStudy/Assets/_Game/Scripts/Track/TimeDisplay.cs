using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KartSystem.Track
{
    /// <summary>
    ///     A class to display information about a particular racer's timings.  WARNING: This class uses strings and creates a
    ///     small amount of garbage each frame.
    /// </summary>
    public class TimeDisplay : MonoBehaviour
    {
        /// <summary>
        ///     The different information that can be displayed on screen.
        /// </summary>
        public enum DisplayOptions
        {
            /// <summary>
            ///     Displays the total time of the current race.
            /// </summary>
            Race,

            /// <summary>
            ///     Displays the time for all complete and non-complete laps.
            /// </summary>
            AllLaps,

            /// <summary>
            ///     Displays the time for all the complete laps.
            /// </summary>
            FinishedLaps,

            /// <summary>
            ///     Displays the time for all the complete laps and the current lap.
            /// </summary>
            FinishedAndCurrentLaps,

            /// <summary>
            ///     Displays the time for the current lap.
            /// </summary>
            CurrentLap,

            /// <summary>
            ///     Displays the time for the best lap since the session started.
            /// </summary>
            SessionBestLap,

            /// <summary>
            ///     Displays the time for the best race since the session started.
            /// </summary>
            SessionBestRace,

            /// <summary>
            ///     Displays the time for the best lap ever.
            /// </summary>
            HistoricalBestLap,

            /// <summary>
            ///     Displays the time for the best race ever.
            /// </summary>
            HistoricalBestRace
        }


        [Tooltip("The timings to be displayed and the order to display them.")]
        public List<DisplayOptions> initialDisplayOptions = new();

        [Tooltip("A reference to the track manager.")]
        public TrackManager trackManager;

        [Tooltip("A reference to the TextMeshProUGUI to display the information.")]
        public TextMeshProUGUI textComponent;

        [Tooltip("A reference to the racer to display the information for.")]
        [RequireInterface(typeof(IRacer))]
        public Object initialRacer;

        private List<Action> _displayCalls = new();
        private IRacer _racer;
        private StringBuilder _stringBuilder = new(0, 300);

        private void Awake()
        {
            for (var i = 0; i < initialDisplayOptions.Count; i++)
                switch (initialDisplayOptions[i])
                {
                    case DisplayOptions.Race:
                        _displayCalls.Add(DisplayRaceTime);
                        break;
                    case DisplayOptions.AllLaps:
                        _displayCalls.Add(DisplayAllLapTimes);
                        break;
                    case DisplayOptions.FinishedLaps:
                        _displayCalls.Add(DisplayFinishedLapTimes);
                        break;
                    case DisplayOptions.FinishedAndCurrentLaps:
                        _displayCalls.Add(DisplayFinishedAndCurrentLapTimes);
                        break;
                    case DisplayOptions.CurrentLap:
                        _displayCalls.Add(DisplayCurrentLapTime);
                        break;
                    case DisplayOptions.SessionBestLap:
                        _displayCalls.Add(DisplaySessionBestLapTime);
                        break;
                    case DisplayOptions.SessionBestRace:
                        _displayCalls.Add(DisplaySessionBestRaceTime);
                        break;
                    case DisplayOptions.HistoricalBestLap:
                        _displayCalls.Add(DisplayHistoricalBestLapTime);
                        break;
                    case DisplayOptions.HistoricalBestRace:
                        _displayCalls.Add(DisplayHistoricalBestRaceTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            _racer = (IRacer)initialRacer;
        }

        private void Update()
        {
            _stringBuilder.Clear();

            foreach (Action displayCall in _displayCalls)
            {
                displayCall.Invoke();
            }

            textComponent.text = _stringBuilder.ToString();
        }

        private void DisplayRaceTime()
        {
            _stringBuilder.AppendLine($"Total: {_racer.GetRaceTime():.##}");
        }

        private void DisplayAllLapTimes()
        {
            var lapTimes = _racer.GetLapTimes();
            var lapTotal = trackManager.raceLapTotal;

            for (var i = 0; i < lapTotal; i++)
            {
                _stringBuilder.Append("Lap ");
                _stringBuilder.Append(i + 1);
                _stringBuilder.Append(": ");

                if (i < lapTimes.Count)
                    _stringBuilder.AppendFormat(lapTimes[i].ToString(".##"));
                else
                    _stringBuilder.Append("--:--");

                _stringBuilder.Append('\n');
            }
        }

        private void DisplayFinishedLapTimes()
        {
            var lapTimes = _racer.GetLapTimes();
            for (var i = 0; i < lapTimes.Count; i++)
            {
                _stringBuilder.Append("Lap ");
                _stringBuilder.Append(i + 1);
                _stringBuilder.Append(": ");
                _stringBuilder.Append(lapTimes[i].ToString(".##"));
                _stringBuilder.Append('\n');
            }
        }

        private void DisplayFinishedAndCurrentLapTimes()
        {
            var lapTimes = _racer.GetLapTimes();
            for (var i = 0; i < lapTimes.Count; i++)
            {
                _stringBuilder.Append("Lap ");
                _stringBuilder.Append(i + 1);
                _stringBuilder.Append(": ");
                _stringBuilder.Append(lapTimes[i].ToString(".##"));
                _stringBuilder.Append('\n');
            }

            var currentLapTime = _racer.GetLapTime();
            if (Mathf.Approximately(currentLapTime, 0f))
                return;

            _stringBuilder.Append("Lap ");
            _stringBuilder.Append(lapTimes.Count + 1);
            _stringBuilder.Append(": ");
            _stringBuilder.Append(currentLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        private void DisplayCurrentLapTime()
        {
            var currentLapTime = _racer.GetLapTime();
            if (Mathf.Approximately(currentLapTime, 0f))
                return;

            _stringBuilder.Append("Current: ");
            _stringBuilder.Append(currentLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        private void DisplaySessionBestLapTime()
        {
            var bestLapTime = trackManager.SessionBestLap;
            if (Mathf.Approximately(bestLapTime, -1f))
                return;

            _stringBuilder.Append("Session Best Lap: ");
            _stringBuilder.Append(bestLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        private void DisplaySessionBestRaceTime()
        {
            var bestLapTime = trackManager.SessionBestRace;
            if (Mathf.Approximately(bestLapTime, -1f))
                return;

            _stringBuilder.Append("Session Best Race: ");
            _stringBuilder.Append(bestLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        private void DisplayHistoricalBestLapTime()
        {
            var bestLapTime = trackManager.HistoricalBestLap;
            if (Mathf.Approximately(bestLapTime, -1f))
                return;

            _stringBuilder.Append("Best Lap Ever: ");
            _stringBuilder.Append(bestLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        private void DisplayHistoricalBestRaceTime()
        {
            var bestLapTime = trackManager.HistoricalBestRace;
            if (Mathf.Approximately(bestLapTime, -1f))
                return;

            _stringBuilder.Append("Best Race Ever: ");
            _stringBuilder.Append(bestLapTime.ToString(".##"));
            _stringBuilder.Append('\n');
        }

        /// <summary>
        ///     Call this function to change what information is currently being displayed.  This causes a GCAlloc.
        /// </summary>
        /// <param name="newDisplay">A collection of the new options for the display.</param>
        /// <exception cref="ArgumentOutOfRangeException">One or more of the display options is not valid.</exception>
        public void RebindDisplayOptions(List<DisplayOptions> newDisplay)
        {
            _displayCalls.Clear();

            for (var i = 0; i < newDisplay.Count; i++)
                switch (newDisplay[i])
                {
                    case DisplayOptions.Race:
                        _displayCalls.Add(DisplayRaceTime);
                        break;
                    case DisplayOptions.AllLaps:
                        _displayCalls.Add(DisplayAllLapTimes);
                        break;
                    case DisplayOptions.FinishedLaps:
                        _displayCalls.Add(DisplayFinishedLapTimes);
                        break;
                    case DisplayOptions.FinishedAndCurrentLaps:
                        _displayCalls.Add(DisplayFinishedAndCurrentLapTimes);
                        break;
                    case DisplayOptions.CurrentLap:
                        _displayCalls.Add(DisplayCurrentLapTime);
                        break;
                    case DisplayOptions.SessionBestLap:
                        _displayCalls.Add(DisplaySessionBestLapTime);
                        break;
                    case DisplayOptions.SessionBestRace:
                        _displayCalls.Add(DisplaySessionBestRaceTime);
                        break;
                    case DisplayOptions.HistoricalBestLap:
                        _displayCalls.Add(DisplayHistoricalBestLapTime);
                        break;
                    case DisplayOptions.HistoricalBestRace:
                        _displayCalls.Add(DisplayHistoricalBestRaceTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        /// <summary>
        ///     Call this function to change the racer about which the information is being displayed.
        /// </summary>
        public void RebindRacer(IRacer newRacer)
        {
            _racer = newRacer;
        }
    }
}