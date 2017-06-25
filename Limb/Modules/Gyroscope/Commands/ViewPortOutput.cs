using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Limb.Modules.Gyroscope.ViewModels;

namespace Limb.Modules.Gyroscope.Commands
{
    [CommandDefinition]
    public class ViewPortOutputCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Gyroscope.PortOutput";

        public override string Name => CommandName;

        public override string Text => "_Port Output";

        public override string ToolTip => "Port output";
    }

    [CommandHandler]
    public class ViewPortOutputCommandHandler : CommandHandlerBase<ViewPortOutputCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewPortOutputCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<PortOutputViewModel>();
            return TaskUtility.Completed;
        }
    }
}
