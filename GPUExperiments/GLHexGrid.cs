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
            GenerateData();
            _gl.Load += glControl1_Load;
            _gl.Paint += glControl1_Paint;
            _gl.Resize += glControl1_Resize;

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += OnTimerOnTick;
            _timer.Enabled = true;
        }

        int sideCount = 6;
        private float[] _hexData;
        private float[] _colors;
        private void GenerateData()
        {
            _hexData = new float[(sideCount + 2) * 2];
            _colors = new float[(sideCount + 2) * 3];
            _hexData[0] = 0f;
            _hexData[1] = 0f;
            _colors[0] = 1f;
            _colors[1] = 1f;
            _colors[2] = 1f;
            float radius = 0.5f;
            float angle = 0;
            float angleStep = (float)(Math.PI * 2.0) / sideCount;
            for (int i = 0; i <= sideCount; i++)
            {
                int index = i + 1;
                _hexData[index * 2 + 0] = (float)Math.Sin(angle) * radius;
                _hexData[index * 2 + 1] = (float)Math.Cos(angle) * radius;
                _colors[index * 3 + 0] = i / (float)sideCount;
                _colors[index * 3 + 1] = 0.2f;
                _colors[index * 3 + 2] = 0.5f;
                angle += angleStep;
            }
        }
        private int _vao;
        private int _vbo;
        private int _color;
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _hexData.Length * sizeof(float), _hexData, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

            _color = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _color);
            GL.BufferData(BufferTarget.ArrayBuffer, _colors.Length * sizeof(float), _colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            
            _shader = new Shader("Shaders/shaderSimple.vert", "Shaders/shaderSimple.frag");
          
            _loaded = true;
            _gl.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, sideCount + 2);
            
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
