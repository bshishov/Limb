using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using SharpDX;

namespace Limb.Modules.Gyroscope
{
    public class SerialGyroscopeConnector : IGyroscopeConnector
    {
        public bool IsActive => _isRunning;
        public string PortName { get; set; } = "COM3";
        public int BaudRate { get; set; } = 115200;

        private SerialPort _serialPort;
        private Thread _thread;
        private bool _isRunning;

        private Vector3 _a;
        private Vector3 _g;
        private Vector3 _m;

        public SerialGyroscopeConnector()
        {
        }

        public void Connect()
        {
            Stop();

            _serialPort = new SerialPort(PortName, BaudRate);

            _thread = new Thread(ReadTask);
            _serialPort.Open();
            _isRunning = true;
            _thread.Start();
            //_serialPort.DataReceived += SerialPortOnDataReceived;
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            throw new NotImplementedException();
        }

        private void ReadTask()
        {
            while (_isRunning)
            {
                try
                {
                    var line = _serialPort.ReadLine();
                    var data = line.Split('\t');
                    if (data.Length > 10)
                    {
                        _a.Z = -Convert.ToSingle(data[1]);
                        _a.Y = -Convert.ToSingle(data[2]);
                        _a.X = Convert.ToSingle(data[3]);

                        _g.Z = -Convert.ToSingle(data[4]);
                        _g.Y = -Convert.ToSingle(data[5]);
                        _g.X = Convert.ToSingle(data[6]);

                        _m.Z = -Convert.ToSingle(data[7]);
                        _m.Y = -Convert.ToSingle(data[8]);
                        _m.X = Convert.ToSingle(data[9]);
                    }
                }
                catch (IOException exception)
                {
                    _isRunning = false;
                    return;
                }
            }
        }

        public void Stop()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                //_serialPort.DataReceived -= SerialPortOnDataReceived;
                _isRunning = false;
                _serialPort.Close();
            }
        }

        public Vector3 GetAccelerometerData(int gyroscopeId)
        {
            return _a;
        }

        public Vector3 GetGyroscopeData(int gyroscopeId)
        {
            return _g;
        }

        public Vector3 GetMagnetometerData(int gyroscopeId)
        {
            return _m;
        }
    }
}
