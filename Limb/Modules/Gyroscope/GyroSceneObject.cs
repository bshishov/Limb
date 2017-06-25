using Limb.Modules.Scene;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Limb.Modules.Gyroscope
{
    public class GyroSceneObject : ISceneObject
    {
        public string Name => $"Gyroscope {_gyroscope.Id}";

        private readonly Gyroscope _gyroscope;
        private GeometricPrimitive _primitive;
        private BasicEffect _effect;

        public GyroSceneObject(Gyroscope gyroscope)
        {
            _gyroscope = gyroscope;
            
        }

        public void OnLoad(GraphicsDevice context)
        {
            _effect = new BasicEffect(context);
            _primitive = GeometricPrimitive.Cube.New(context);
            _effect.EnableDefaultLighting();
        }

        public void Draw(GraphicsDevice context, Matrix view, Matrix projection)
        {
            if (_effect == null)
            {
                OnLoad(context);
            }

            var rotation = _gyroscope.GetRotation();
            _effect.World = Matrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.Translation(_gyroscope.GetPosition() * 0.0001f);
            _effect.View = view;
            _effect.Projection = projection;
            _primitive.Draw(_effect);
        }
    }
}
