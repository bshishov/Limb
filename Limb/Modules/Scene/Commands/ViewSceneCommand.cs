using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Limb.Modules.Scene.ViewModels;

namespace Limb.Modules.Scene.Commands
{
    [CommandDefinition]
    public class ViewSceneCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demos.SceneViewer";

        public override string Name => CommandName;

        public override string Text => "_3D Scene";

        public override string ToolTip => "3D Scene";
    }

    [CommandHandler]
    public class ViewSceneCommandHandler : CommandHandlerBase<ViewSceneCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewSceneCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument(new SceneViewModel());
            return TaskUtility.Completed;
        }
    }
}
