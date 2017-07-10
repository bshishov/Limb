using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limb.Modules.Gyroscope
{
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
}
