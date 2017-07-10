using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Port;

namespace Limb.Modules.Gyroscope
{
    class SerialConnection: IGyroConnection, IDisposable
    {
        private SerialPort _port;

        public SerialConnection(string portName)
        {
            _port = new SerialPort();
            AdjustPort(portName);
        }

        public void AdjustPort(string name, int baudRate = 9600, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            _port.PortName = name;
            _port.BaudRate = baudRate;
            _port.Parity = parity;
            _port.StopBits = stopBits;
        }

        public void Connect()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
            }
        }

        public void CloseConnection()
        {
            if (_port.IsOpen)
            {
                _port.Close();
            }
        }

        public byte[] Receive()
        {
            // one byte for id, 2 bytes per two axis, start and stop byte
            byte[] chunk = new byte[9];
            _port.Read(chunk, 0, 9);
            return chunk;
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
