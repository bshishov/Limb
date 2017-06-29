using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Gemini.Modules.Inspector;
using Gemini.Modules.PropertyGrid;

namespace Limb.Modules.Gyroscope.ViewModels
{
    public class ConnectorViewModel : PropertyChangedBase
    {
        public string Name => $"{_connector.GetType().Name}";

        public IEnumerable<GyroscopeViewModel> Gyroscopes
        {
            get
            {
                return _deviceManager?.Gyroscopes
                    .Where(g => g.Connector.Equals(_connector))
                    .Select(g => new GyroscopeViewModel(g));
            }
        }

        private readonly DeviceManager _deviceManager;
        private readonly IGyroscopeConnector _connector;

        public ConnectorViewModel(IGyroscopeConnector connector)
        {
            _deviceManager = IoC.Get<DeviceManager>();
            _connector = connector;
        }

        public void OpenEditor()
        {
            var propertyGrid = IoC.Get<IPropertyGrid>();
            propertyGrid.SelectedObject = _connector;

            IoC.Get<IInspectorTool>().SelectedObject =
                new InspectableObjectBuilder().WithObjectProperties(_connector, x => true).ToInspectableObject();
        }

        public void StartListening()
        {
            _connector?.Connect();
        }

        public void StopListening()
        {
            _connector?.Stop();
        }
    }
}
