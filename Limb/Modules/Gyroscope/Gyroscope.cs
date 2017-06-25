using System.Diagnostics;
using SharpDX;

namespace Limb.Modules.Gyroscope
{
    public class Gyroscope
    {
        public int Id { get; set; } = 0;
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
            var delta = (_stopwatch.ElapsedMilliseconds - _lastPosTime) * 0.001f;
            _lastPosTime = _stopwatch.ElapsedMilliseconds;
            _velocity += Connector.GetAccelerometerData(Id) / 16f * delta;
            _position += _velocity * delta;
            //return Connector.GetAccelerometerData(Id);
            return Vector3.Zero;
        }


        public Vector3 GetRotation()
        {
            var delta = (_stopwatch.ElapsedMilliseconds - _lastRotTime) * 0.001f;
            _lastRotTime = _stopwatch.ElapsedMilliseconds;
            _rotation += Connector.GetGyroscopeData(Id) / 1000f * delta;
            //return Connector.GetGyroscopeData(Id) / 200f * delta;
            //return Connector.GetAccelerometerData(Id) * delta / 16;
            return _rotation;
        }
    }
}
