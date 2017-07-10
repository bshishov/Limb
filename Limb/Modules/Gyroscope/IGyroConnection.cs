using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limb.Modules.Gyroscope
{
    public interface IGyroConnection
    {
        void Connect();
        void CloseConnection();
        byte[] Receive();
    }
}
