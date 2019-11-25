using System;
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
        private int cols = 8;
        private int rows = 8;

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
                    sides.Add(rnd.Next(3, 10));
                    GenVAO(index);
                }
            }

            int _textureId = ShaderBase.CreateTexture(TextureUnit.Texture0, Resources.glTest5);
            _vfShader = new VertexFragmentShader("Shaders/shaderSimple.vert", "Shaders/shaderSimple.frag");
            _computeShader = new ComputeShader("Shaders/shaderSimple.comp");

            GL.GetInteger((GetPName)All.MaxComputeWorkGroupCount, out var workGroupCount);
            GL.GetInteger((GetPName)All.MaxComputeWorkGroupSize, out var workGroupSize);
            int workGroupInvocations = GL.GetInteger((GetPName)All.MaxComputeWorkGroupInvocations);

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

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();
            GL.DispatchCompute(256/16,256/16,1);
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);//ShaderImageAccessBarrierBit);

            _vfShader.Use();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int index = x + y * cols;
                    var view = Matrix4.Identity;
                    // view *= Matrix4.CreateTranslation(-0.5f, 0.5f, 0);
                    view *= Matrix4.CreateScale(1f/cols, 1f/rows, 1);
                    float xOffset = y % 2 == 0 ? 2f : 1f;
                    view.M14 = x / (float)cols * 2.0f - 1f + xOffset/cols - 0.5f/cols;
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
