using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GPUExperiments.Common
{
    public abstract class ShaderBase
    {
        public int ProgramId;
        protected readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();

        protected int CreateProgram(params int[] shaderIds)
        {
            ProgramId = GL.CreateProgram();
            AttachShadersById(ProgramId, shaderIds);
            AddUniforms();
            return ProgramId;
        }

        protected int CreateShaderFromPath(string path, ShaderType shaderType)
        {
            var src = LoadSource(path);
            var shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, src);
            CompileShader(shaderId);
            return shaderId;
        }
        protected void AddUniforms()
        {
            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(ProgramId, i, out _, out _);
                var location = GL.GetUniformLocation(ProgramId, key);
                UniformLocations.Add(key, location);
            }
        }
        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(ProgramId, attribName);
        }
        public void SetUniformInt(string name, int data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform1(UniformLocations[name], data);
        }
        public void SetUniformFloat(string name, float data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform1(UniformLocations[name], data);
        }
        public void SetUniformMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(ProgramId);
            GL.UniformMatrix4(UniformLocations[name], true, ref data);
        }
        public void SetUniformVector3(string name, Vector3 data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform3(UniformLocations[name], data);
        }
        public static int CreateTexture(TextureUnit textureUnit, int width, int height)
        {
            int result = GL.GenTexture();

            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, result);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            int unitIndex = textureUnit - TextureUnit.Texture0;
            GL.BindImageTexture(unitIndex, result,
                0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba8);

            return result;
        }

        public static int CreateTexture(TextureUnit textureUnit, Bitmap bmp)
        {
            int result = GL.GenTexture();

            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, result);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            int unitIndex = textureUnit - TextureUnit.Texture0;
            GL.BindImageTexture(unitIndex, result, 0, false, 0,
                TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);

            return result;
        }


        protected static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                string msg = GL.GetShaderInfoLog(shader);
                Debug.WriteLine("Shader compile error: " + msg);
                throw new Exception("Shader compile error: " + shader.ToString());
            }
        }
        protected static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                Debug.WriteLine("Linker compile error: " + GL.GetProgramInfoLog(program));
                throw new Exception("Linker compile error: " + program);
            }
        }
        protected static void AttachShadersById(int programHandle, params int[] shaderIds)
        {
            for (int i = 0; i < shaderIds.Length; i++)
            {
                GL.AttachShader(programHandle, shaderIds[i]);

            }

            LinkProgram(programHandle);

            for (int i = 0; i < shaderIds.Length; i++)
            {
                GL.DetachShader(programHandle, shaderIds[i]);
                GL.DeleteShader(shaderIds[i]);
            }
        }
        protected static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
