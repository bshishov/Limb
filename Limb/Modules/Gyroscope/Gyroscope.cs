using System;
using System.Diagnostics;
using SharpDX;

namespace Limb.Modules.Gyroscope
{
    public class Gyroscope
    {
        public int Id { get; set; } = 0;
        public Vector3 AccelerometerOffset { get; set; } = Vector3.Zero;
        public Vector3 GyroscopeOffset { get; set; } = Vector3.Zero;
        public bool UsePosition { get; set; }
        public float AccelScale { get; set; } = 1f;

        public IGyroscopeConnector Connector { get; private set; }

        private Vector3 _position;
        private Vector3 _velocity;
        private Vector3 _rotation;
        private Stopwatch _stopwatch = new Stopwatch();
        private float _lastRotTime;
        private float _lastPosTime;

        public Gyroscope(IGyroscopeConnector connector)
        {
            Connector = connector;
            _stopwatch.Start();
        }

        public Vector3 GetPosition()
        {
            return Vector3.UnitX * 5f;

            if (!UsePosition)
                return Vector3.Zero;

            var delta = (_stopwatch.ElapsedMilliseconds - _lastPosTime) * 0.001f;
            _lastPosTime = _stopwatch.ElapsedMilliseconds;
            _velocity += Connector.GetAccelerometerData(Id) * delta;
            _position += _velocity * delta;
            return _position;
        }


        public Vector3 GetRotation()
        {
            var deltaTime = (_stopwatch.ElapsedMilliseconds - _lastRotTime) * 0.001f;
            _lastRotTime = _stopwatch.ElapsedMilliseconds;

            
            var rawGyro = GyroscopeOffset + Connector.GetGyroscopeData(Id)/1000f * deltaTime;
            var rawAccel = Connector.GetAccelerometerData(Id) * AccelScale;// * deltaTime * 0.000001f;

            var roll = Math.Atan2(rawAccel.Y, rawAccel.Z);
            var pitch = Math.Atan2(-rawAccel.X, Math.Sqrt(rawAccel.Y*rawAccel.Y + rawAccel.Z*rawAccel.Z));

            //return new Vector3((float)roll, (float)pitch, 0);


            _rotation += Connector.GetGyroscopeData(Id) / 1000f * deltaTime;
            //return Connector.GetGyroscopeData(Id) / 200f * delta;
            //return Connector.GetAccelerometerData(Id) * delta / 16;
            return _rotation;
        }
    }
}
