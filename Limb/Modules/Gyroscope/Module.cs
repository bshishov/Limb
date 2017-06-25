using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Limb.Modules.Gyroscope.Commands;

namespace Limb.Modules.Gyroscope
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        public static MenuItemDefinition ViewSceneViewerMenuItem =
            new CommandMenuItemDefinition<ViewPortOutputCommandDefinition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);
    }
}
