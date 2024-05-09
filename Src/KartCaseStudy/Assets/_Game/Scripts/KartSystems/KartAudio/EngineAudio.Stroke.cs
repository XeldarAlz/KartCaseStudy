using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KartSystem.KartSystems
{
    public partial class EngineAudio
    {
        /// <summary>
        ///     Represents audio data for a single stroke of an engine (2 strokes per revolution)
        /// </summary>
        [Serializable]
        public struct Stroke
        {
            public AudioClip clip;
            [Range(0, 1)] public float gain;
            internal float[] Buffer;
            internal int Position;

            internal void Reset()
            {
                Position = 0;
            }

            internal float Sample()
            {
                if (Position < Buffer.Length)
                {
                    var s = Buffer[Position];
                    Position++;
                    return s * gain;
                }

                return 0;
            }

            internal void Init()
            {
                //if no clip is available use a noisy sine wave as a place holder.
                //else initialise buffer of samples from clip data.
                if (clip == null)
                {
                    Buffer = new float[4096];
                    for (var i = 0; i < Buffer.Length; i++)
                        Buffer[i] = Mathf.Sin(i * (1f / 44100) * 440) + Random.Range(-1, 1) * 0.05f;
                }
                else
                {
                    Buffer = new float[clip.samples];
                    clip.GetData(Buffer, 0);
                }
            }
        }
    }
}