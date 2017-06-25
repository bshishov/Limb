using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace Limb.Modules.Gyroscope.Commands
{
    [CommandDefinition]
    public class AddConnectorCommand : CommandDefinition
    {
        public override string Name => "AddConnector";
        public override string Text => "Add connector";
        public override string ToolTip => "Add new hardware connector";
    }
}
