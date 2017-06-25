using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Limb.Modules.Scene
{
    public interface ISceneObject
    {
        string Name { get; }
        void OnLoad(GraphicsDevice context);
        void Draw(GraphicsDevice context, Matrix view, Matrix projection);
    }
}
