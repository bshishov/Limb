using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Gyro
{
    class Program
    {
        static void Main(string[] args)
        {

            SerialConnection testConnection = new SerialConnection("COM4");
            testConnection.Connect();

            while (true)
            {   
                Console.WriteLine("Type 'quit' to stop");
                string command = Console.ReadLine();

                if (command == "quit")
                {
                    break;
                }
                else
                {
                    byte[] message = testConnection.Receive();
                    GyroData receivedData = Protocol.Parse(message);
                    String report = String.Format("Gyroscope {0}: \t mag(x)={1} \t acc(x)={2} \t gyro(x)={3}",
                        receivedData.GyroId, receivedData.Mag(0), receivedData.Acc(0), receivedData.Gyro(0));
                    Console.WriteLine(report);
                }

            }
        }
    }

    interface IGyroConnection
    {
        void Connect();
        void CloseConnection();
        byte[] Receive();
    }

    class SerialConnection: IGyroConnection
    {
        private SerialPort _port;

        public SerialConnection(string portName)
        {
            _port = new SerialPort();
            AdjustPort(portName);
        }

        private void AdjustPort(string name, int baudRate=9600, Parity parity=Parity.None, StopBits stopBits=StopBits.One)
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
            if (!_port.IsOpen)
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
    }

    public class GyroData
    {
        private int[] _mag;
        private int[] _acc;
        private int[] _gyro;

        public int GyroId
        { get; set; }

        public int Mag(int axis)
        {
            return _mag[axis];
        }

        public void SetMag(int axis, int value)
        {
            _mag[axis] = value;
        }

        public int Acc(int axis)
        {
            return _acc[axis];
        }

        public void SetAcc(int axis, int value)
        {
            _acc[axis] = value;
        }

        public int Gyro(int axis)
        {
            return _gyro[axis];
        }
        public void SetGyro(int axis, int value)
        {
            _gyro[axis] = value;
        }

        public GyroData()
        {
            // initialized but does not assigned with any gyro
            GyroId = -1;
            _mag = new int[3];
            _acc = new int[3];
            _gyro = new int[3];
        }
    }

    class Protocol
    {
        private static byte[] _dataBuffer;

        private const byte StartByte = 0x0;
        private const byte StopByte = 0xff;
        private const byte AdditionalByte = 0x55;

        

        public static GyroData Parse(byte[] newData)
        {
            if (newData[0] == StartByte && newData[newData.Length] == StopByte)
            {
                return ParseCompleteMessage(newData);
            }
            else
            {
                AddToDataBuffer(newData);
                return TryToParseExistingData();
            }     
        }

        private static GyroData ParseCompleteMessage(byte[] message)
        {
            // According to https://habrahabr.ru/post/174115/
            // MSB first

            GyroData data = new GyroData();
            data.GyroId = message[1];

            // skip start byte and GyroId
            int currentIndex = 2;
            // need to correctly assign values
            int iterations = 0;

            while (currentIndex < message.Length)
            {
                int value = 0;

                if (message[currentIndex] == StartByte)
                {
                    switch (message[currentIndex + 1])
                    {
                        case StartByte:
                            value = MakeIntFromBytes(StartByte, message[currentIndex + 2]);
                            break;

                        case AdditionalByte:
                            value = MakeIntFromBytes(StopByte, message[currentIndex + 2]);
                            break;
                    }
                    currentIndex += 3;
                }
                else
                {
                    value = MakeIntFromBytes(message[currentIndex], message[currentIndex + 1]);
                    currentIndex += 2;
                }
                AssignValue(data, value, iterations);
                iterations++;
            }

            return data;
        }

        private static GyroData TryToParseExistingData()
        {
            int startBytePosition = Array.IndexOf(_dataBuffer, StartByte);
            int stopBytePisition = -1;
            if (startBytePosition >= 0)
            {
                stopBytePisition = Array.IndexOf(_dataBuffer, StopByte, startBytePosition);
            }

            if (startBytePosition != -1 && stopBytePisition != -1)
            {
                int messageLength = stopBytePisition - startBytePosition;
                byte[] message = new byte[messageLength];

                Array.ConstrainedCopy(_dataBuffer, startBytePosition, message, 0, messageLength);
                CutDataBuffer(stopBytePisition);
                    
                return ParseCompleteMessage(message);
            }
            else
            {
                // TODO: maybe it would be better to raise an exception
                return new GyroData();
            }
        }

        private static void AddToDataBuffer(byte[] newData)
        {
            if (_dataBuffer == null)
            {
                _dataBuffer = newData;
            }
            else
            {
                int newBufferLength = _dataBuffer.Length + newData.Length;
                byte[] newDataBuffer = new byte[newBufferLength];

                Array.ConstrainedCopy(_dataBuffer, 0, newDataBuffer, 0, _dataBuffer.Length);
                Array.ConstrainedCopy(newData, 0, newDataBuffer, _dataBuffer.Length-1, newData.Length);
                _dataBuffer = newDataBuffer;
            }
        }

        private static void CutDataBuffer(int cutIndex)
        {
            // we can loose some information here, but if we cut a part of dataBuffer from the middle,
            // it might lead to the corruption of some other part of data
            int bytesToCopy = _dataBuffer.Length - cutIndex;
            byte[] newDataBuffer = new byte[bytesToCopy];

            Array.ConstrainedCopy(_dataBuffer, cutIndex, newDataBuffer, 0, bytesToCopy);
            _dataBuffer = newDataBuffer;
        }

        private static int MakeIntFromBytes(byte Msb, byte Lsb)
        {
            int result = Msb << 8;
            result &= Lsb;
            return result;
        }

        private static void AssignValue(GyroData data, int value, int iteration)
        {
            // we need 3 iterations for one device.
            // sequence in received data is: Mag, Acc, Gyro 
            // 0 is X, 1 is Y, 3 is Z
            if (iteration > 5)
            {
                data.SetGyro(iteration - 6, value);
            }
            else if (iteration > 2)
            {
                data.SetAcc(iteration - 3, value);
            }
            else
            {
                data.SetMag(iteration, value);
            }
        }
    }
}
