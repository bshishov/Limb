using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limb.Modules.Gyroscope
{
    public interface IProtocol
    {
        GyroData Parse(byte[] newData);
    }
}
