using UnityEngine;

namespace KartSystem.KartSystems
{
    /// <summary>
    ///     A class which produces audio based on the speed that the kart is going.
    /// </summary>
    public partial class EngineAudio : MonoBehaviour
    {
        [RequireInterface(typeof(IKartInfo))] public Object kartInfo;

        [Range(0, 1)] public float RPM;

        [Space] [Tooltip("The minimum possible RPM of the engine.")]
        public float minRPM = 900;

        [Tooltip("The maximum possible RPM of the engine.")]
        public float maxRPM = 10000;

        [Space] [Tooltip("Increases randomness in engine audio.")]
        public float lumpyCamFactor = 0.05f;

        [Space] [Tooltip("Volume when at mininum RPM")]
        public float minVolume = 0.2f;

        [Tooltip("Volume when at maximum RPM")]
        public float maxVolume = 1.2f;

        [Space] [Tooltip("Smoothing of wave when a new stroke begins.")]
        public float strokeDamping = 0.1f;

        [Space] [Tooltip("Audio configuration for each engine stroke.")]
        public Stroke intake, compression, combustion, exhaust;

        [Tooltip("Map RPM to RPM^3")] public bool usePow = false;

        private IKartInfo _kartInfo;
        private float _nextStrokeTime;
        private float _time;
        private float _secondsPerSample;
        private int _stroke;
        private float[] _randomBuffer;
        private float _deltaRPM;
        private float _lastRPM;
        private float _lastSampleL, _lastSampleR;
        private float _damper = 1f;
        private float _volume = 1;

        private void Awake()
        {
            _kartInfo = kartInfo as IKartInfo;
            _randomBuffer = new float[97];
            for (var i = 0; i < _randomBuffer.Length; i++)
                _randomBuffer[i] = Random.Range(-1, 1);
            intake.Init();
            compression.Init();
            combustion.Init();
            exhaust.Init();

            _stroke = 0;
            _time = 0;
            _secondsPerSample = 1f / AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            RPM = _kartInfo.LocalSpeed / _kartInfo.CurrentStats.topSpeed;
            _deltaRPM = RPM - _lastRPM;

            //damp the movement of _lastRPM
            _lastRPM = Mathf.Lerp(_lastRPM, RPM, Time.deltaTime * 100);
            _volume = Time.timeScale < 1 ? 0 : 1;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (channels != 2)
                return;
            var r = usePow ? _lastRPM * _lastRPM * _lastRPM : _lastRPM;
            var gain = Mathf.Lerp(minVolume, maxVolume, r);

            //4 strokes per revolution
            var strokeDuration = 1f / (Mathf.Lerp(minRPM, maxRPM, r) / 60f * 2);

            for (var i = 0; i < data.Length; i += 2)
            {
                _time += _secondsPerSample;

                //a small random value use to mimic a "lumpy cam".
                var rnd = _randomBuffer[i % 97] * lumpyCamFactor;

                //is it time for the next stroke?
                if (_time >= _nextStrokeTime)
                {
                    switch (_stroke)
                    {
                        case 0:
                            intake.Reset();
                            break;
                        case 1:
                            compression.Reset();
                            break;
                        case 2:
                            combustion.Reset();
                            break;
                        case 3:
                            exhaust.Reset();
                            break;
                    }

                    //increase the stroke counter
                    _stroke++;
                    if (_stroke >= 4) _stroke = 0;

                    //next stroke time has lump cam factor applied when rpm is decreasing (throttling down).
                    _nextStrokeTime += strokeDuration + strokeDuration * rnd * (_deltaRPM < 0 ? 1 : 0);

                    //damping resets every stroke, helps removes clicks and improves transition between strokes.
                    _damper = 0;
                }

                var sampleL = 0f;
                var sampleR = 0f;

                //In a 4 cylinder engine, all strokes would be playing simulataneously.
                switch (_stroke)
                {
                    case 0:
                        sampleL += intake.Sample() * rnd;
                        sampleR += compression.Sample();
                        sampleL += combustion.Sample();
                        sampleR += exhaust.Sample();
                        break;
                    case 1:
                        sampleR += intake.Sample();
                        sampleL += compression.Sample() * rnd;
                        sampleR += combustion.Sample();
                        sampleL += exhaust.Sample();
                        break;
                    case 2:
                        sampleR += intake.Sample();
                        sampleR += compression.Sample();
                        sampleL += combustion.Sample() * rnd;
                        sampleL += exhaust.Sample();
                        break;
                    case 3:
                        sampleL += intake.Sample();
                        sampleL += compression.Sample();
                        sampleR += combustion.Sample();
                        sampleR += exhaust.Sample() * rnd;
                        break;
                }

                _damper += strokeDamping;
                if (_damper > 1) _damper = 1;

                //smooth out samples between strokes
                sampleL = _lastSampleL + (sampleL - _lastSampleL) * _damper;
                sampleR = _lastSampleR + (sampleR - _lastSampleR) * _damper;
                sampleL = Mathf.Clamp(sampleL * gain, -1, 1);
                sampleR = Mathf.Clamp(sampleR * gain, -1, 1);
                data[i + 0] += sampleL + sampleR * 0.75f;
                data[i + 0] *= _volume;
                data[i + 1] += sampleR + sampleL * 0.75f;
                data[i + 1] *= _volume;
                _lastSampleL = sampleL;
                _lastSampleR = sampleR;
            }
        }
    }
}