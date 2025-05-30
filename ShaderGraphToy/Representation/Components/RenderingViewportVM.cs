using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using ShaderGraphToy.Graphics;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Utilities.Common;

namespace ShaderGraphToy.Representation.Components
{
    internal class RenderingViewportVM : VmBase
    {
        private bool _renderingPaused = false;


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
                    _nvidiaOptimusStatus = RenderingDeviceManager.IsNvapiActive ? "renderingViewport_nvapiStatusYes" : "renderingViewport_nvapiStatusNo";

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
        private string[] _uniforms = [ "u_Time", "u_Resolution" ];



        public void OpenTkControl_Ready()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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

            string vertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders/plane.vert");
            string fragPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders/lava.frag");
            _shaderProgram = new Shader(vertPath, fragPath);
            _shaderProgram.Use();

            _timer = new Stopwatch();
            _timer.Start();

            OpenTkRendererAPI.RegisterRenderer(this);
        }

        public void OpenTkControl_OnRender(TimeSpan delta)
        {
            GL.ClearColor(0.15f, 0.15f, 0.15f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateRenderingStats(delta);

            if (_shaderProgram != null)
            {
                _shaderProgram.Use();
                SetUniforms();
            }

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        private void SetUniforms()
        {
            if (_uniforms.Contains("u_Time"))
            {
                double timeVal = _timer!.Elapsed.TotalSeconds;
                int timeLoc = GL.GetUniformLocation(_shaderProgram!.Handle, "u_Time");
                GL.Uniform1(timeLoc, (float)timeVal);
            }
            if (_uniforms.Contains("u_Resolution"))
            {
                int resLoc = GL.GetUniformLocation(_shaderProgram!.Handle, "u_Resolution");
                GL.Uniform2(resLoc, (float)ViewportWidth, (float)ViewportHeight);
            }
            if (_uniforms.Contains("u_Mouse"))
            {

            }

            if (_uniforms.Contains("texture0"))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _tex);
            }
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
            _renderingPaused = !_renderingPaused;
        }


        public void ChangeFragmentShader(string code, string[] uniforms)
        {
            string vertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders/plane.vert");

            if (_shaderProgram != null) _shaderProgram!.Dispose();
            _uniforms = uniforms;
            _shaderProgram = new Shader(vertPath, code, false, true);

            _shaderProgram.Use();
            OnBreakRendering();
        }

        private uint _tex;
        public void RegisterTexture(byte[] data, int width, int height)
        {
            GL.GenTextures(1, out _tex);
            GL.BindTexture(TextureTarget.Texture2D, _tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            _shaderProgram!.Use();
            GL.Uniform1(GL.GetUniformLocation(_shaderProgram.Handle, "texture0"), 0);
        }
    }
}
