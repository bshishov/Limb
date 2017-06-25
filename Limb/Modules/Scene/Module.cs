using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Limb.Modules.Scene.Commands;
using Limb.Modules.Scene.ViewModels;

namespace Limb.Modules.Scene
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        public static MenuItemDefinition ViewSceneViewerMenuItem = 
            new CommandMenuItemDefinition<ViewSceneCommandDefinition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);

        public override IEnumerable<IDocument> DefaultDocuments
        {
            get { yield return new SceneViewModel(); }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
