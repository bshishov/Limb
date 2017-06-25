using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Limb.Modules.Scene.ViewModels;
using Gemini.Modules.SharpDX.Controls;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace Limb.Modules.Scene.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        public const float CameraMoveSpeed = 0.1f;
        public const float CameraPanSpeed = 0.01f;
        public const float CameraRotSpeed = 0.003f;

        public ObservableCollection<ISceneObject> Objects = new ObservableCollection<ISceneObject>();

        private BasicEffect _effect;
        private Texture2D _texture;

        // A yaw and pitch applied to the viewport based on input
        private System.Windows.Point _previousPosition;
        private float _yaw;
        private float _pitch;
        private Vector3 _cameraPosition = new Vector3(0, 0, 3);
        private float _cameraPitch;
        private float _cameraYaw;
        private Matrix _camera;
        private bool _lockCursor;

        private GeometricPrimitive[] _geometricPrimitives;
        private int _primitiveIndex;

        public SceneView()
        {
            InitializeComponent();
            Objects.CollectionChanged += ObjectsOnCollectionChanged;
        }

        private void ObjectsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ISceneObject newItem in notifyCollectionChangedEventArgs.NewItems)
                {
                }
            }
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void OnGraphicsControlLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            _effect = new BasicEffect(e.GraphicsDevice)
            {
                View = Matrix.LookAtRH(new Vector3(0, 0, 3), new Vector3(0, 0, 0), Vector3.UnitY),
                World = Matrix.Identity,
                PreferPerPixelLighting = true,
                FogEnabled = true,
                FogStart = 1,
                FogEnd = 100,
            };
            
            _effect.EnableDefaultLighting();

            
            _texture = Texture2D.Load(e.GraphicsDevice, "Modules/Scene/Resources/aima_1.png");
            _effect.Texture = _texture;
            _effect.TextureEnabled = true;

            //var content = new SharpDX.Toolkit.Content.ContentManager(IoC.Get<IServiceProvider>());
            //content.Resolvers.Add(new FileSystemContentResolver("C:\\Users\\boris\\Documents\\stealthshifter\\Assets\\Models\\"));

            //var path = "C:\\Users\\boris\\Documents\\stealthshifter\\Assets\\Models\\assets.fbx";
            //var path = "Z:\\Общие Проекты\\monkey.x";
            //_model = content.Load<Model>("Z:\\Общие Проекты\\monkey.x");
            //_model = content.Load<Model>("assets.fbx");

            //_model = SharpDX.Toolkit.Graphics.Model.Load(e.GraphicsDevice, File.Open(path, FileMode.Open), name => _texture);

            _geometricPrimitives = new[]
            {
                GeometricPrimitive.Cube.New(e.GraphicsDevice),
                GeometricPrimitive.Cylinder.New(e.GraphicsDevice),
                GeometricPrimitive.GeoSphere.New(e.GraphicsDevice),
                GeometricPrimitive.Teapot.New(e.GraphicsDevice),
                GeometricPrimitive.Torus.New(e.GraphicsDevice),
                GeometricPrimitive.Plane.New(e.GraphicsDevice, 10f, 10f)

            };
            _primitiveIndex = 0;

            _yaw = 0.5f;
            _pitch = 0.3f;
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void OnGraphicsControlDraw(object sender, DrawEventArgs e)
        {
            e.GraphicsDevice.Clear(Color.Black);

            var position = ((SceneViewModel)DataContext).Position;
            _camera = Matrix.RotationYawPitchRoll(-_cameraYaw, -_cameraPitch, 0f) * Matrix.Translation(_cameraPosition);
            _effect.View = Matrix.Invert(_camera);



            //_effect.View = Matrix.LookAtRH(_cameraPosition, Vector3.Zero, Vector3.UnitY);
            _effect.World = Matrix.RotationYawPitchRoll(_yaw, _pitch, 0f)
                * Matrix.Translation(position);
            _effect.Projection =
                Matrix.PerspectiveFovRH((float)Math.PI / 4.0f,
                    (float)e.GraphicsDevice.BackBuffer.Width / e.GraphicsDevice.BackBuffer.Height,
                    0.1f, 100.0f);

           

            
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (Keyboard.IsKeyDown(Key.W))
                    _cameraPosition += _camera.Forward * CameraMoveSpeed;

                if (Keyboard.IsKeyDown(Key.S))
                    _cameraPosition += _camera.Backward * CameraMoveSpeed;

                if (Keyboard.IsKeyDown(Key.A))
                    _cameraPosition += _camera.Left * CameraMoveSpeed;

                if (Keyboard.IsKeyDown(Key.D))
                    _cameraPosition += _camera.Right * CameraMoveSpeed;

                if (Keyboard.IsKeyDown(Key.LeftShift))
                    _cameraPosition += Vector3.UnitY * CameraMoveSpeed;

                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    _cameraPosition -= Vector3.UnitY * CameraMoveSpeed;

                //GraphicsControl.Invalidate();
            }

            //_geometricPrimitives[_primitiveIndex].Draw(_effect);
            
            foreach (var sceneObject in Objects)
            {
                sceneObject.Draw(e.GraphicsDevice, _effect.View, _effect.Projection);
            }
        }

        private void OnGraphicsControlUnloadContent(object sender, GraphicsDeviceEventArgs e)
        {
            _texture?.Dispose();
            foreach (var primitive in _geometricPrimitives)
                primitive.Dispose();
            _effect.Dispose();
        }

        private void OnGraphicsControlMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            var delta = new Vector2((float)(position.X - _previousPosition.X), (float)(position.Y - _previousPosition.Y));

            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _yaw += delta.X * .01f;
                _pitch += delta.Y * .01f;
                GraphicsControl.Invalidate();
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                _cameraYaw += delta.X * CameraRotSpeed;
                _cameraPitch +=  delta.Y * CameraRotSpeed;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _cameraPosition += _camera.Left * CameraPanSpeed * delta.X;
                _cameraPosition += _camera.Up * CameraPanSpeed * delta.Y;
            }

            if (_lockCursor)
            {
                var scrPos = GraphicsControl.PointToScreen(_previousPosition);
                SetCursorPos((int) scrPos.X, (int) scrPos.Y);
            }
            else
            {
                _previousPosition = position;
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void OnGraphicsControlHwndLButtonDown(object sender, MouseEventArgs e)
        {
            _previousPosition = e.GetPosition(this);
            GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
            this.Cursor = Cursors.None;
            _lockCursor = true;
        }

        private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            GraphicsControl.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
            _lockCursor = false;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var newIndex = _primitiveIndex + 1;
            newIndex %= _geometricPrimitives.Length;
            _primitiveIndex = newIndex;
        }
    }
}
