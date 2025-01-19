using GUI.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для RenderingViewport.xaml
    /// </summary>
    public partial class RenderingViewport : UserControl
    {
        private Shader _shaderProgram;

        private int VAO, VBO, EBO;
        private Stopwatch _timer;
        private long _totalDelta = 0;
        private long _framesRendered = 0;


        public RenderingViewport()
        {
            InitializeComponent();

            SetupOpenGlControl();
            //DataContext = new RenderingViewportVM();
        }



        private void SetupOpenGlControl()
        {
            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 3
            };
            openTkControl.Start(settings);
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateRenderingStats(delta);

            _shaderProgram.Use();

            double timeVal = _timer.Elapsed.TotalSeconds;
            int timeLoc = GL.GetUniformLocation(_shaderProgram.Handle, "Time");
            GL.Uniform1(timeLoc, (float)timeVal);
            int resLoc = GL.GetUniformLocation(_shaderProgram.Handle, "Resolution");
            GL.Uniform2(resLoc, (float)openTkControl.ActualWidth, (float)openTkControl.ActualHeight);

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private void ToStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_timer.IsRunning) _timer.Restart();
            else _timer.Reset();
        }

        private void Pause_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_timer.IsRunning)
            {
                _timer.Stop();
                pauseButton.Source = new BitmapImage(new Uri("../Images/play_icon.png", UriKind.Relative));
            }
            else
            {
                _timer.Start();
                pauseButton.Source = new BitmapImage(new Uri("../Images/pause_icon.png", UriKind.Relative));
            }
        }

        private void OpenTkControl_Ready()
        {
            float[] verts =
            {
                 1.0f,  1.0f, 0.0f,  // top right
                 1.0f, -1.0f, 0.0f,  // bottom right
                -1.0f, -1.0f, 0.0f,  // bottom left
                -1.0f,  1.0f, 0.0f   // top left
            };
            uint[] inds =
            {
                0, 1, 3,
                1, 2, 3
            };

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




        private void UpdateRenderingStats(TimeSpan delta)
        {
            _totalDelta += delta.Milliseconds;
            _framesRendered++;

            if (_timer.ElapsedMilliseconds % 60 < 10)
            {
                timeCounter.Text = $"{_timer.Elapsed.Seconds}.{_timer.Elapsed.Milliseconds / 10} sec";
                resolutionDisplay.Text = $"{(int)openTkControl.ActualWidth} x {(int)openTkControl.ActualHeight}";
            }
            if (_totalDelta >= 1000)
            {
                fpsCounter.Text = _framesRendered + " fps";
                _totalDelta = 0;
                _framesRendered = 0;
            }
        }

    }
}
