using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraphToy.Graphics
{
    public class Shader : IDisposable
    {
        private bool _disposed = false;

        public int Handle { get; private set; }


        public Shader(string vertexPath, string fragmentPath)
        {
            (int vert, int frag) = CompileShader(vertexPath, fragmentPath);
            LinkShader(vert, frag);
            FreeUpResources(vert, frag);
        }


        private (int, int) CompileShader(string vertexPath, string fragmentPath)
        {
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);

            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0) throw new Exception($"Vertex shader compilation error: {GL.GetShaderInfoLog(vertexShader)}");

            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0) throw new Exception($"Fragment shader compilation error: {GL.GetShaderInfoLog(fragmentShader)}");

            return (vertexShader, fragmentShader);
        }

        private void LinkShader(int vert, int frag)
        {
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0) throw new Exception($"Shader linking error: {GL.GetProgramInfoLog(Handle)}");
        }

        private void FreeUpResources(int vert, int frag)
        {
            GL.DetachShader(Handle, vert);
            GL.DetachShader(Handle, frag);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }


        public void Use() => GL.UseProgram(Handle);


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteProgram(Handle);
                _disposed = true;
            }
        }

        ~Shader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

