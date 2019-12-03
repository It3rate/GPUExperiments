using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using GPUExperiments.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Timer = System.Windows.Forms.Timer;

namespace GPUExperiments
{
    public class GLPolyDraw
    {
        private int _textureId;
        private VertexFragmentShader _vfShader;
        private ComputeShader _computeShader;
        private GLControl _gl;
        private bool _loaded;
        private System.Timers.Timer _timer;
        private static readonly Random rnd = new Random();

        public GLPolyDraw(GLControl control)
        {
            _gl = control;
            Initialize();
        }

        private void Initialize()
        {
            _gl.Load += glControl1_Load;
            _gl.Paint += glControl1_Paint;
            _gl.Resize += glControl1_Resize;

            _timer = new System.Timers.Timer();
            _timer.Elapsed += Tick;
            _timer.Interval = 60;
            _timer.Enabled = true;
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            _gl.Invalidate();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            int maxVertexUniformComponents = GL.GetInteger(GetPName.MaxVertexUniformComponents);
            int maxTextureBufferSize = GL.GetInteger(GetPName.MaxTextureBufferSize);

            GL.ClearColor(0.2f, 0.0f, 0.0f, 1f);
            GL.Enable(EnableCap.DepthTest);

            _vfShader = new VertexFragmentShader("Shaders/polyShader.vert", "Shaders/polyShader.frag");
            _computeShader = new ComputeShader("Shaders/polyShader.comp");

            uint vertexCount = _triangleCount * sideCount * 3;

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (int)vertexCount * PolyVertex.ByteSize, IntPtr.Zero,
                BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, (int)BufferSlots.Vertexes, _vbo);

            _vba = GL.GenVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BindVertexArray(_vba);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, PolyVertex.ByteSize,
                PolyVertex.LocationOffset);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, PolyVertex.ByteSize,
                PolyVertex.ColorOffset);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            _indirect = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, _indirect);
            var pts = new DrawArraysIndirectCommand(vertexCount, 1); // vertCount, instCount 
            GL.BufferData(BufferTarget.DrawIndirectBuffer, DrawArraysIndirectCommand.Stride, ref pts,
                BufferUsageHint.DynamicDraw);

			program = new ProgramStateBuffer();
			AddColors(program.FloatSeries);
			program.BindAll();

            _loaded = true;
            _gl.Invalidate();
        }

        private void AddColors(FloatSeriesBuffer seriesBuffer)
        {
            var data1 = new List<Color>(){
                Color.Red, Color.Orange, Color.Yellow, Color.GreenYellow, Color.Green, Color.Blue, Color.BlueViolet, Color.Violet};
            var data2 = new List<Color>(){
                Color.DarkRed, Color.DarkOrange, Color.DarkGoldenrod, Color.DarkGreen, Color.DarkCyan, Color.DarkBlue, Color.DarkViolet, Color.DarkMagenta};
			var data3 = new List<Color>();
			for (int i = 0; i < 100; i++)
			{
				data3.Add(Color.FromArgb(1, rnd.Next(128,255), rnd.Next(1, 255), (int)(i / 100f * 255)));
			}
			seriesBuffer.AddSeries(data1);
			seriesBuffer.AddSeries(data2);
			seriesBuffer.AddSeries(data3);
        }

        private ProgramStateBuffer program;

        uint sideCount = 6;
        private uint _triangleCount = 22 * 22;
        private int _indirect;
        private int _vbo;
        private int _vba;
        private uint _counter = 0;

        //private int _floatSeries;
        //private FloatSeriesBuffer floatBuffer;
        //private int _pointers;

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();
			program.BindUpdate(_counter++);
			GL.DispatchCompute(_triangleCount, 1, 1);

            //GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);

            _vfShader.Use();
            GL.BindVertexArray(_vba);
            GL.DrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero);

            _gl.SwapBuffers();
        }



        private void glControl1_Resize(object sender, EventArgs e)
        {
        }
    }
}


public struct PolyVertex
{
    public Vector4 Location;
    public Vector4 Color;
    public PolyVertex(Vector4 location, Vector4 color)
    {
        Location = location;
        Color = color;
    }
    public PolyVertex(float x, float y, Color color)
    {
        Location = new Vector4(x, y, 0, 1f);
        Color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }
    public static int LocationOffset => 0;
    public static int ColorOffset => (int)(sizeof(float) * 4);
    public static int Size => 8;
    public static int ByteSize => sizeof(float) * Size;
}

public struct DrawArraysIndirectCommand
{
    public uint Count;
    public uint PrimCount;
    public uint First;
    public uint ReservedMustBeZero;

    public DrawArraysIndirectCommand(uint count, uint primCount, uint first = 0)
    {
        Count = count;
        PrimCount = primCount;
        First = first;
        ReservedMustBeZero = 0;
    }

    public static int Size => 4;
    public static int Stride => sizeof(uint) * Size;
}

