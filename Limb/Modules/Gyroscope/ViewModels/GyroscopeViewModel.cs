using System;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Limb.Modules.Scene.ViewModels;

namespace Limb.Modules.Gyroscope.ViewModels
{
    public class GyroscopeViewModel
    {
        public string Name => $"Gyroscope {_gyroscope.Id}";

        private readonly Gyroscope _gyroscope;

        public GyroscopeViewModel(Gyroscope gyroscope)
        {
            _gyroscope = gyroscope;
        }

        public void Open3DViewer()
        {
            var shell = IoC.Get<IShell>();
            var scene = new SceneViewModel();            
            shell.OpenDocument(scene);
            scene.AddSceneObject(new GyroSceneObject(_gyroscope));
        }
    }
}