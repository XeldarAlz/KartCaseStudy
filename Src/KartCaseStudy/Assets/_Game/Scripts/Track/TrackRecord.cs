using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace KartSystem.Track
{
    /// <summary>
    ///     A serializable record for the time on a track.
    /// </summary>
    [Serializable]
    public class TrackRecord
    {
        /// <summary>
        ///     The name of the track this record belongs to.
        /// </summary>
        public string trackName;

        /// <summary>
        ///     The number of laps this record is for.
        /// </summary>
        public int laps;

        /// <summary>
        ///     The time of this record.
        /// </summary>
        public float time;

        /// <summary>
        ///     The name of the racer who recorded this time.
        /// </summary>
        public string name;

        private const float DefaultTime = float.PositiveInfinity;
        private const string FolderName = "BinaryTrackRecordData";
        private const string FileExtension = ".dat";

        /// <summary>
        ///     Set all the information in a record.
        /// </summary>
        /// <param name="track">The new name of the track.</param>
        /// <param name="lapCount">The new lap count.</param>
        /// <param name="racer">The new racer whose name will be recorded.</param>
        /// <param name="newTime">The new time for the record.</param>
        public void SetRecord(string track, int lapCount, IRacer racer, float newTime)
        {
            trackName = track;
            laps = lapCount;
            name = racer.GetName();
            time = newTime;
        }

        /// <summary>
        ///     Creates a TrackRecord with default values.
        /// </summary>
        public static TrackRecord CreateDefault()
        {
            var defaultRecord = new TrackRecord();
            defaultRecord.time = DefaultTime;
            return defaultRecord;
        }

        /// <summary>
        ///     Saves a record using a file name based on the track name and number of laps.
        /// </summary>
        public static void Save(TrackRecord record)
        {
            var folderPath = Path.Combine(Application.persistentDataPath, FolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var dataPath = Path.Combine(folderPath, record.trackName + record.laps + FileExtension);

            var binaryFormatter = new BinaryFormatter();

            using var fileStream = File.Open(dataPath, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(fileStream, record);
        }

        /// <summary>
        ///     Finds and loads a TrackRecord file.
        /// </summary>
        /// <param name="track">The name of the track to be loaded.</param>
        /// <param name="lapCount">The number of laps of the record to be loaded.</param>
        /// <returns>The loaded record.</returns>
        public static TrackRecord Load(string track, int lapCount)
        {
            var folderPath = Path.Combine(Application.persistentDataPath, FolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var dataPath = Path.Combine(folderPath, track + lapCount + FileExtension);

            var binaryFormatter = new BinaryFormatter();

            using var fileStream = File.Open(dataPath, FileMode.OpenOrCreate);
            if (fileStream.Length == 0)
                return CreateDefault();

            try
            {
                var loadedRecord = binaryFormatter.Deserialize(fileStream) as TrackRecord;

                if (loadedRecord == null)
                    return CreateDefault();
                return loadedRecord;
            }
            catch (Exception)
            {
                return CreateDefault();
            }
        }
    }
}