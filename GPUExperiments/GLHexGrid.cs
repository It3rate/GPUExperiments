using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments
{
    public class GLHexGrid
    {
	    private Shader _shader;
        private GLControl _gl;
        Timer _timer;
        private bool _loaded;

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
        
        private void GenerateData(int sideCount, out float[] polygon, out float[] colors)
        {
            polygon = new float[(sideCount + 2) * 2];
            colors = new float[(sideCount + 2) * 3];
            polygon[0] = 0f;
            polygon[1] = 0f;
            colors[0] = 1f;
            colors[1] = 1f;
            colors[2] = 1f;
            float radius = 1f;
            float angle = 0;
            float angleStep = (float)(Math.PI * 2.0) / sideCount;
            for (int i = 0; i <= sideCount; i++)
            {
                int index = i + 1;
                polygon[index * 2 + 0] = (float)Math.Sin(angle) * radius;
                polygon[index * 2 + 1] = (float)Math.Cos(angle) * radius;
                colors[index * 3 + 0] = i / (float)sideCount;
                colors[index * 3 + 1] = 0.2f;
                colors[index * 3 + 2] = 0.5f;
                angle += angleStep;
            }
        }
        private int _vao1;
        private int _vao2;
        private int _vbo;
        private int _color;
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);
            GL.Enable(EnableCap.DepthTest);

            GenVAO(0);
            GenVAO(1);
            GenVAO(2);

            _shader = new Shader("Shaders/shaderSimple.vert", "Shaders/shaderSimple.frag");
          
            _loaded = true;
            _gl.Invalidate();
        }

        private int[] sides = {6, 8, 7};
        List<int> vaos = new List<int>();
        private void GenVAO(int index)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            vaos.Add(vao);

            GenerateData(sides[index], out var hexData, out var colors);
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, hexData.Length * sizeof(float), hexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

            _color = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _color);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    var view = Matrix4.Identity;
                   // view *= Matrix4.CreateTranslation(-0.5f, 0.5f, 0);
                    view *= Matrix4.CreateScale(0.2f);
                    view.M14 = x / 2f - 0.5f;
                    var tm = Matrix4.CreateTranslation(1f, 1f, 0);
                    //view += tm;
                    _shader.SetMatrix4("view", view);

                    GL.BindVertexArray(vaos[x]);
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, sides[x] + 2);
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
