using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ToolBars.Models;
using Limb.Modules.Gyroscope.Commands;

namespace Limb.Modules.Gyroscope.ViewModels
{
    [Export(typeof(PortOutputViewModel))]
    public class PortOutputViewModel : Tool
    {
        
        public static ToolBarDefinition Toolbar = new ToolBarDefinition(0, "Device Manager");

        [Export]
        public static ToolBarItemGroupDefinition ToolbarGroup = new ToolBarItemGroupDefinition(Toolbar, 0);

        [Export]
        public static ToolBarItemDefinition AddConnector = new CommandToolBarItemDefinition<AddConnectorCommand>(ToolbarGroup, 0, ToolBarItemDisplay.IconAndText);
        

        public IEnumerable<ConnectorViewModel> Connectors => 
            _deviceManager?.Connectors.Select(c => new ConnectorViewModel(c));


        public override PaneLocation PreferredLocation => PaneLocation.Right;

        [Import]
        private DeviceManager _deviceManager;

        public PortOutputViewModel()
        {
            DisplayName = "Devices";
            this.ToolBarDefinition = Toolbar;
        }
    }
}
