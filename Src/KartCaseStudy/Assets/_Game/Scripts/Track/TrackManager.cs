using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KartSystem.KartSystems;
using UnityEngine;

namespace KartSystem.Track
{
    /// <summary>
    ///     A MonoBehaviour to deal with all the time and positions for the racers.
    /// </summary>
    public class TrackManager : MonoBehaviour
    {
        [Tooltip("The name of the track in this scene.  Used for track time records.  Must be unique.")]
        public string trackName;

        [Tooltip("Number of laps for the race.")]
        public int raceLapTotal;

        [Tooltip(
            "All the checkpoints for the track in the order that they should be completed starting with the start/finish line checkpoint.")]
        public List<Checkpoint> checkpoints = new();

        [Tooltip("Reference to an object responsible for repositioning karts.")]
        public KartRepositioner kartRepositioner;

        private readonly Dictionary<IRacer, Checkpoint> _racerNextCheckpoints = new(16);
        private readonly TrackRecord _sessionBestLap = TrackRecord.CreateDefault();
        private readonly TrackRecord _sessionBestRace = TrackRecord.CreateDefault();
        private TrackRecord _historicalBestLap;
        private TrackRecord _historicalBestRace;

        public bool IsRaceRunning { get; private set; }

        /// <summary>
        ///     Returns the best lap time recorded this session.  If no record is found, -1 is returned.
        /// </summary>
        public float SessionBestLap =>
            _sessionBestLap is { time: < float.PositiveInfinity } ? _sessionBestLap.time : -1f;

        /// <summary>
        ///     Returns the best race time recorded this session.  If no record is found, -1 is returned.
        /// </summary>
        public float SessionBestRace =>
            _sessionBestRace is { time: < float.PositiveInfinity } ? _sessionBestRace.time : -1f;

        /// <summary>
        ///     Returns the best lap time ever recorded.  If no record is found, -1 is returned.
        /// </summary>
        public float HistoricalBestLap =>
            _historicalBestLap is { time: < float.PositiveInfinity } ? _historicalBestLap.time : -1f;

        /// <summary>
        ///     Returns the best race time ever recorded.  If no record is found, -1 is returned.
        /// </summary>
        public float HistoricalBestRace =>
            _historicalBestRace is { time: < float.PositiveInfinity } ? _historicalBestRace.time : -1f;

        private void Awake()
        {
            if (checkpoints.Count < 3)
                Debug.LogWarning("There are currently " + checkpoints.Count +
                                 " checkpoints set on the Track Manager.  A minimum of 3 is recommended but kart control will not be enabled with 0.");

            _historicalBestLap = TrackRecord.Load(trackName, 1);
            _historicalBestRace = TrackRecord.Load(trackName, raceLapTotal);
        }

        private void OnEnable()
        {
            foreach (var checkpoint in checkpoints) checkpoint.OnKartHitCheckpoint += CheckRacerHitCheckpoint;
        }

        private void OnDisable()
        {
            foreach (var checkpoint in checkpoints) checkpoint.OnKartHitCheckpoint -= CheckRacerHitCheckpoint;
        }

        private void Start()
        {
            if (checkpoints.Count == 0) return;

            var allRacerArray = FindObjectsOfType<Object>().Where(x => x is IRacer).ToArray();

            foreach (var currentRacer in allRacerArray)
                if (currentRacer is IRacer racer)
                {
                    _racerNextCheckpoints.Add(racer, checkpoints[0]);
                    racer.DisableControl();
                }
        }

        /// <summary>
        ///     Starts the timers and enables control of all racers.
        /// </summary>
        public void StartRace()
        {
            IsRaceRunning = true;

            foreach (var racerNextCheckpoint in _racerNextCheckpoints)
            {
                racerNextCheckpoint.Key.EnableControl();
                racerNextCheckpoint.Key.UnpauseTimer();
            }
        }

        /// <summary>
        ///     Stops the timers and disables control of all racers, also saves the historical records.
        /// </summary>
        public void StopRace()
        {
            IsRaceRunning = false;

            foreach (var racerNextCheckpoint in _racerNextCheckpoints)
            {
                racerNextCheckpoint.Key.DisableControl();
                racerNextCheckpoint.Key.PauseTimer();
            }

            TrackRecord.Save(_historicalBestLap);
            TrackRecord.Save(_historicalBestRace);
        }

        private void CheckRacerHitCheckpoint(IRacer racer, Checkpoint checkpoint)
        {
            if (!IsRaceRunning)
            {
                StartCoroutine(CallWhenRaceStarts(racer, checkpoint));
                return;
            }

            if (_racerNextCheckpoints.ContainsKeyValuePair(racer, checkpoint))
            {
                _racerNextCheckpoints[racer] = checkpoints.GetNextInCycle(checkpoint);
                RacerHitCorrectCheckpoint(racer, checkpoint);
            }
            else
            {
                RacerHitIncorrectCheckpoint(racer, checkpoint);
            }
        }

        private IEnumerator CallWhenRaceStarts(IRacer racer, Checkpoint checkpoint)
        {
            while (!IsRaceRunning) yield return null;

            CheckRacerHitCheckpoint(racer, checkpoint);
        }

        private void RacerHitCorrectCheckpoint(IRacer racer, Checkpoint checkpoint)
        {
            if (checkpoint.isStartFinishLine)
            {
                var racerCurrentLap = racer.GetCurrentLap();

                if (racerCurrentLap > 0)
                {
                    var lapTime = racer.GetLapTime();

                    if (_sessionBestLap.time > lapTime) _sessionBestLap.SetRecord(trackName, 1, racer, lapTime);

                    if (_historicalBestLap.time > lapTime) _historicalBestLap.SetRecord(trackName, 1, racer, lapTime);

                    if (racerCurrentLap == raceLapTotal)
                    {
                        var raceTime = racer.GetRaceTime();

                        if (_sessionBestRace.time > raceTime)
                            _sessionBestRace.SetRecord(trackName, raceLapTotal, racer, raceTime);

                        if (_historicalBestRace.time > raceTime)
                            _historicalBestLap.SetRecord(trackName, raceLapTotal, racer, raceTime);

                        // racer.DisableControl();
                        // racer.PauseTimer();
                    }
                }

                // if (CanEndRace()) StopRace();
                racer.HitStartFinishLine();
            }
        }

        private bool CanEndRace()
        {
            return _racerNextCheckpoints.All(racerNextCheckpoint => racerNextCheckpoint.Key.GetCurrentLap() >= raceLapTotal);
        }

        private void RacerHitIncorrectCheckpoint(IRacer racer, Checkpoint checkpoint)
        {
            // No implementation required
        }

        /// <summary>
        ///     This function should be called when a kart gets stuck or falls off the track.
        ///     It will find the last checkpoint the kart went through and reposition it there.
        /// </summary>
        /// <param name="movable">The movable representing the kart.</param>
        public void ReplaceMovable(IMovable movable)
        {
            var racer = movable.GetRacer();

            if (racer == null)
                return;

            var nextCheckpoint = _racerNextCheckpoints[racer];
            var lastCheckpointIndex = (checkpoints.IndexOf(nextCheckpoint) + checkpoints.Count - 1) % checkpoints.Count;
            var lastCheckpoint = checkpoints[lastCheckpointIndex];

            var isControlled = movable.IsControlled();
            movable.DisableControl();
            kartRepositioner.OnRepositionComplete += ReenableControl;

            kartRepositioner.Reposition(lastCheckpoint, movable, isControlled);
        }

        private void ReenableControl(IMovable movable, bool doEnableControl)
        {
            if (doEnableControl)
                movable.EnableControl();
            kartRepositioner.OnRepositionComplete -= ReenableControl;
        }
    }
}