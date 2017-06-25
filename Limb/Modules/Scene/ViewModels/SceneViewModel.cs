using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Limb.Modules.Scene.Views;
using SharpDX;

namespace Limb.Modules.Scene.ViewModels
{
    [Export(typeof(SceneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SceneViewModel : Document
    {
        public override bool ShouldReopenOnStart => false;

        private Vector3 _position;
        private SceneView _view;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }

        public SceneViewModel()
        {
            DisplayName = "3D Scene";
        }

        private List<ISceneObject> _pending = new List<ISceneObject>();

        public void AddSceneObject(ISceneObject obj)
        {
            if (_view == null)
            {
                _pending.Add(obj);
            }
            else
            {
                _view.Objects.Add(obj);
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _view = this.GetView() as SceneView;
            foreach (var sceneObject in _pending)
            {
                _view.Objects.Add(sceneObject);
            }
        }
    }
}
