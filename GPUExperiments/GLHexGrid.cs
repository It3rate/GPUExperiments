﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPUExperiments.Common;
using GPUExperiments.Properties;
using GPUExperiments.Shaders;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GPUExperiments
{
    public class GLHexGrid
    {
        private int _textureId;
        private VertexFragmentShader _vfShader;
        private ComputeShader _computeShader;
        private GLControl _gl;
        Timer _timer;
        private bool _loaded;
        private int cols = 6;//2;//
        private int rows = 6;//2;//

        public GLHexGrid(GLControl control)
        {
            _gl = control;
            Initialize();
        }

        private void Initialize()
        {
            _gl.Load += glControl1_Load;
            _gl.Paint += glControl1_Paint;
            _gl.Resize += glControl1_Resize;

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += OnTimerOnTick;
            _timer.Enabled = true;
        }

        private int _vao1;
        private int _vao2;
        private int _vbo;
        private int _color;
        private static readonly Random rnd = new Random();

        List<int> sides = new List<int>();
        List<int> vaos = new List<int>();
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);
            GL.Enable(EnableCap.DepthTest);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int index = x + y * cols;
                    sides.Add(rnd.Next(3, 12));
                    GenVAO(index);
                }
            }
			GenerateBuffer();
			//ShaderProgram.SetupDebugOutput();
            _vfShader = new VertexFragmentShader("Shaders/shaderSimple.vert", "Shaders/shaderSimple.frag");
            _computeShader = new ComputeShader("Shaders/shaderSimple.comp");
            _textureId = ShaderProgram.CreateTexture(TextureUnit.Texture0, Resources.glTest5, TextureAccess.WriteOnly);
            _textureId = ShaderProgram.CreateTexture(TextureUnit.Texture1, Resources.glTest5, TextureAccess.ReadOnly);

            //GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 0, out int xc);
            //GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 1, out int yc);
            //GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 2, out int zc);

            int maxVertexImageUniforms = GL.GetInteger(GetPName.MaxVertexImageUniforms);
            int maxFragmentImageUniforms = GL.GetInteger(GetPName.MaxFragmentImageUniforms);
            int maxColorTextureSamples = GL.GetInteger(GetPName.MaxColorTextureSamples);
            int maxCombinedImageUniforms = GL.GetInteger(GetPName.MaxCombinedImageUniforms);
            int maxCombinedTextureImageUnits = GL.GetInteger(GetPName.MaxCombinedTextureImageUnits);

            _loaded = true;
            _gl.Invalidate();
        }
		
		
        private void GenVAO(int index)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            vaos.Add(vao);

            GenerateData(sides[index], out var hexData, out var colors);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, hexData.Length * sizeof(float), hexData, BufferUsageHint.StreamDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

            _color = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _color);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.StreamDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            var _txtVert = GL.GenBuffer();
            float[] texVerticies =
            {
               // Texture coordinates
               1.0f, 1.0f, // top right
               1.0f, 0.0f, // bottom right
               0.0f, 0.0f, // bottom left
               0.0f, 1.0f  // top left
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, _txtVert);
            GL.BufferData(BufferTarget.ArrayBuffer, texVerticies.Length * sizeof(float), texVerticies, BufferUsageHint.StreamDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);


        }

        private int _bufferId;

        private struct Polygon
        {
	        public int SideCount;
	        public float Radius;
	        public float[] Points;

	        public Polygon(int sideCount, float radius, float[] points)
	        {
		        SideCount = sideCount;
		        Radius = radius;
		        Points = points;
	        }
        }
        private void GenerateBuffer()
        {
			GenerateData(6, out var polygon, out var colors);
			Polygon buffer = new Polygon(6, 0.1f, polygon); 
	        _bufferId = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _bufferId);
			GL.BufferData(BufferTarget.ShaderStorageBuffer, 2 + buffer.Points.Length, polygon, BufferUsageHint.DynamicCopy);
			GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _bufferId);
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }
        private void GenerateData(int sideCount, out float[] polygon, out float[] colors)
        {
            //VertexPosColor[] result = new VertexPosColor[sideCount + 2];
            float radius = (1f - (sideCount - 3f) / 8f) * 0.25f + .95f;
            polygon = new float[(sideCount + 2) * 2];
            colors = new float[(sideCount + 2) * 4];
            polygon[0] = 0f;
            polygon[1] = 0f;
            colors[0] = 1f;
            colors[1] = 1f;
            colors[2] = 1f;
            colors[3] = radius;
            float angle = 0;
            float angleStep = (float)(Math.PI * 2.0) / sideCount;
            float flatTop = rnd.NextDouble() > 0.5 ? angleStep / 2f : 0;
            for (int i = 0; i <= sideCount; i++)
            {
                int index = i + 1;
                polygon[index * 2 + 0] = (float)Math.Sin(angle + flatTop) * radius;
                polygon[index * 2 + 1] = (float)Math.Cos(angle + flatTop) * radius;
                colors[index * 4 + 0] = i / (float)sideCount;
                colors[index * 4 + 1] = 0.2f;
                colors[index * 4 + 2] = 0.5f;
                colors[index * 4 + 3] = radius;
                angle += angleStep;
            }
        }

        private int frame = 0;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();
            //GL.BindImageTexture(0, _textureId, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba8);

            frame++;
            int speed = 128;
            if (frame == speed) frame = 0;
            GL.Uniform1(GL.GetUniformLocation(_computeShader.ProgramId, "roll"), frame / (float)speed);

            GL.DispatchCompute(256,256,1);
            //GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            _vfShader.Use();
            //GL.BindTexture(TextureTarget.ProxyTexture2D, _textureId);



            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int index = x + y * cols;
                    var view = Matrix4.Identity;
                    view *= Matrix4.CreateScale(1f / cols, 1f / rows, 1);
                    float xOffset = y % 2 == 0 ? 2f : 1f;
                    view.M14 = x / (float)cols * 2.0f - 1f + xOffset / cols - 0.5f / cols;
                    view.M24 = y / (float)rows * 2.0f - 1f + 1f / rows;
                    var tm = Matrix4.CreateTranslation(1f, 1f, 0);
                    //view += tm;
                    _vfShader.SetUniformMatrix4("view", view);

                    GL.BindVertexArray(vaos[index]);
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, sides[index] + 2);
                }
            }

            _gl.SwapBuffers();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!_loaded)
                return;
            GL.Viewport(0, 0, _gl.Width, _gl.Height);
        }
        private void OnTimerOnTick(object sender, EventArgs args)
        {
            _gl.Invalidate();
        }

    }
}
