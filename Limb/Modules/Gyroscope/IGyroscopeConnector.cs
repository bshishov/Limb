using SharpDX;

namespace Limb.Modules.Gyroscope
{
    public interface IGyroscopeConnector
    {
        bool IsActive { get; }

        void Connect();
        void Stop();

        Vector3 GetAccelerometerData(int gyroscopeId);
        Vector3 GetGyroscopeData(int gyroscopeId);
        Vector3 GetMagnetometerData(int gyroscopeId);
    }
}