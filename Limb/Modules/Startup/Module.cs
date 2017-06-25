using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Limb.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        public static MenuDefinition Hardware = new MenuDefinition(Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar, 5, "_Hardware");

        [Export]
        public static MenuItemGroupDefinition HardwareMenuGroup = new MenuItemGroupDefinition(Hardware, 0);

        public override void Initialize()
        {
            MainWindow.Title = "Limb";
        }
    }
}
