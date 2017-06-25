using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limb.Modules.Gyroscope
{
    [Export]
    public class DeviceManager
    {
        public readonly List<IGyroscopeConnector> Connectors = new List<IGyroscopeConnector>();
        public readonly List<Gyroscope> Gyroscopes = new List<Gyroscope>();

        public DeviceManager()
        {
            var connector = new SerialGyroscopeConnector();
            Connectors.Add(connector);
            Gyroscopes.Add(new Gyroscope(connector));
            Gyroscopes.Add(new Gyroscope(connector));
        }
    }
}
