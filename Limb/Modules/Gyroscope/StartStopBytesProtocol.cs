using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limb.Modules.Gyroscope
{
    class StartStopBytesProtocol: IProtocol
    {
        private  byte[] _dataBuffer;

        private const byte StartByte = 0x0;
        private const byte StopByte = 0xff;
        private const byte AdditionalByte = 0x55;



        public GyroData Parse(byte[] newData)
        {
            // TOD
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

        private  GyroData ParseCompleteMessage(byte[] message)
        {
            // According to https://habrahabr.ru/post/174115/
            // MSB first

            var data = new GyroData
            {
                GyroId = message[1]
            };
        

            // skip start byte and GyroId
            var currentIndex = 2;
            // need to correctly assign values
            var iterations = 0;

            while (currentIndex < message.Length)
            {
                var value = 0;

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

        private GyroData TryToParseExistingData()
        {
            var startBytePosition = Array.IndexOf(_dataBuffer, StartByte);
            var stopBytePisition = -1;
            if (startBytePosition >= 0)
            {
                stopBytePisition = Array.IndexOf(_dataBuffer, StopByte, startBytePosition);
            }

            if (startBytePosition != -1 && stopBytePisition != -1)
            {
                var messageLength = stopBytePisition - startBytePosition;
                var message = new byte[messageLength];

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

        private void AddToDataBuffer(byte[] newData)
        {
            if (_dataBuffer == null)
            {
                _dataBuffer = newData;
            }
            else
            {
                var newBufferLength = _dataBuffer.Length + newData.Length;
                var newDataBuffer = new byte[newBufferLength];

                Array.ConstrainedCopy(_dataBuffer, 0, newDataBuffer, 0, _dataBuffer.Length);
                Array.ConstrainedCopy(newData, 0, newDataBuffer, _dataBuffer.Length - 1, newData.Length);
                _dataBuffer = newDataBuffer;
            }
        }

        private void CutDataBuffer(int cutIndex)
        {
            // we can loose some information here, but if we cut a part of dataBuffer from the middle,
            // it might lead to the corruption of some other part of data
            var bytesToCopy = _dataBuffer.Length - cutIndex;
            var newDataBuffer = new byte[bytesToCopy];

            Array.ConstrainedCopy(_dataBuffer, cutIndex, newDataBuffer, 0, bytesToCopy);
            _dataBuffer = newDataBuffer;
        }

        private int MakeIntFromBytes(byte Msb, byte Lsb)
        {
            var result = Msb << 8;
            result &= Lsb;
            return result;
        }

        private void AssignValue(GyroData data, int value, int iteration)
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
