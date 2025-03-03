using GUI.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using ShaderGraph.Graphics;

namespace GUI.Components
{
    internal class RenderingViewportVM : INotifyPropertyChanged
    {
        #region PROPS
        private RelayCommand? _pauseClickCommand = null;
        public RelayCommand PauseClickCommand
        {
            get
            {
                _pauseClickCommand ??= new RelayCommand(OnPauseRendering);
                return _pauseClickCommand;
            }
            set
            {
                _pauseClickCommand = value;
                OnPropertyChanged(nameof(PauseClickCommand));
            }
        }

        private RelayCommand? _breakClickCommand = null;
        public RelayCommand BreakClickCommand
        {
            get
            {
                _breakClickCommand ??= new RelayCommand(OnBreakRendering);
                return _breakClickCommand;
            }
            set
            {
                _breakClickCommand = value;
                OnPropertyChanged(nameof(BreakClickCommand));
            }
        }

        private bool _renderingPaused = false;
        private BitmapImage _pauseButtonImage = new(new Uri("../Images/pause_icon.png", UriKind.Relative));
        public BitmapImage PauseButtonImage
        {
            get => _pauseButtonImage;
            set
            {
                _pauseButtonImage = value;
                OnPropertyChanged(nameof(PauseButtonImage));
            }
        }

        private string _timeDisplay = "0.01";
        public string TimeDisplay
        {
            get => _timeDisplay;
            set
            {
                _timeDisplay = value;
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

        private string _fpsDisplay = "120 fps";
        public string FpsDisplay
        {
            get => _fpsDisplay;
            set
            {
                _fpsDisplay = value;
                OnPropertyChanged(nameof(FpsDisplay));
            }
        }

        private string _resolutionDisplay = "800 x 450";
        public string ResolutionDisplay
        {
            get => _resolutionDisplay;
            set
            {
                _resolutionDisplay = value;
                OnPropertyChanged(nameof(ResolutionDisplay));
            }
        }

        private string _nvidiaOptimusStatus = string.Empty;
        public string NvidiaOptimusStatus
        {
            get
            {
                if (_nvidiaOptimusStatus == string.Empty)
                    _nvidiaOptimusStatus = ((App)App.Current).IsNvapiActive ? "Принудительный выбор GPU" : "Недоступен";

                return _nvidiaOptimusStatus;
            }
            set
            {
                _nvidiaOptimusStatus = value;
                OnPropertyChanged(nameof(NvidiaOptimusStatus));
            }
        }

        public double ViewportWidth { get; set; }
        public double ViewportHeight { get; set; }
        #endregion
        
        private Shader? _shaderProgram;
        private int VAO, VBO, EBO;
        private Stopwatch? _timer;
        private long _totalDelta = 0;
        private long _framesRendered = 0;



        public void OpenTkControl_Ready()
        {
            float[] verts =
            [
                 1.0f,  1.0f, 0.0f,  // top right
                 1.0f, -1.0f, 0.0f,  // bottom right
                -1.0f, -1.0f, 0.0f,  // bottom left
                -1.0f,  1.0f, 0.0f   // top left
            ];
            uint[] inds =
            [
                0, 1, 3,
                1, 2, 3
            ];

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, inds.Length * sizeof(float), inds, BufferUsageHint.StaticDraw);

            string vertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders/triangleShader.vert");
            //string fragPath = "Shaders/triangleShader.frag";
            string fragPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders/HappyJumpingShader.frag");
            _shaderProgram = new Shader(vertPath, fragPath);
            _shaderProgram.Use();

            _timer = new Stopwatch();
            _timer.Start();
        }

        public void OpenTkControl_OnRender(TimeSpan delta)
        {
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateRenderingStats(delta);

            _shaderProgram!.Use();

            double timeVal = _timer!.Elapsed.TotalSeconds;
            int timeLoc = GL.GetUniformLocation(_shaderProgram.Handle, "Time");
            GL.Uniform1(timeLoc, (float)timeVal);
            int resLoc = GL.GetUniformLocation(_shaderProgram.Handle, "Resolution");
            GL.Uniform2(resLoc, (float)ViewportWidth, (float)ViewportHeight);

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private void OnBreakRendering()
        {
            if (_timer!.IsRunning) _timer.Restart();
            else _timer.Reset();
        }

        private void OnPauseRendering()
        {
            if (_timer!.IsRunning) _timer.Stop();
            else _timer.Start();

            TogglePauseButtonImage();
        }


        private void UpdateRenderingStats(TimeSpan delta)
        {
            _totalDelta += delta.Milliseconds;
            _framesRendered++;

            if (_timer!.ElapsedMilliseconds % 60 < 10)
            {
                TimeDisplay = $"{_timer.Elapsed.Seconds}.{_timer.Elapsed.Milliseconds / 10} сек";
                ResolutionDisplay = $"{(int)ViewportWidth} x {(int)ViewportHeight}";
            }
            if (_totalDelta >= 1000)
            {
                FpsDisplay = _framesRendered + " fps";
                _totalDelta = 0;
                _framesRendered = 0;
            }
        }

        private void TogglePauseButtonImage()
        {
            if (_renderingPaused) PauseButtonImage = new BitmapImage(new Uri("../Images/pause_icon.png", UriKind.Relative));
            else PauseButtonImage = new BitmapImage(new Uri("../Images/play_icon.png", UriKind.Relative));

            _renderingPaused = !_renderingPaused;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
