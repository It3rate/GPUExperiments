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
using GPUExperiments.Common.Buffers;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Timer = System.Windows.Forms.Timer;

namespace GPUExperiments
{
    public class GLPolyDraw
    {
        private VertexFragmentShader _vfShader;
        private ComputeShader _computeShader;
        private GLControl _gl;

        private bool _loaded;
        private System.Timers.Timer _timer;
        private static readonly Random rnd = new Random();

        private ProgramStateBuffer _program;
        uint sideCount = 6;
        private uint _triangleCount = 22 * 22;
        private int _indirect;
        private uint _counter;

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
            uint vertexCount = _triangleCount * sideCount * 3;

            GL.ClearColor(0.2f, 0.0f, 0.0f, 1f);
            GL.Enable(EnableCap.DepthTest);
            _vfShader = new VertexFragmentShader("Shaders/polyShader.vert", "Shaders/polyShader.frag");
            _computeShader = new ComputeShader("Shaders/polyShader.comp");

			_program = new ProgramStateBuffer();
			AddColors(_program.FloatSeries);
            uint capacity = (uint)Math.Pow(2, (int)Math.Log(vertexCount, 2) + 1);
			_program.VertexBuffer.AdjustCapacity(capacity);
			_program.BindAll();

			_indirect = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, _indirect);
            var pts = new DrawArraysIndirectCommand(vertexCount, 1);
            GL.BufferData(BufferTarget.DrawIndirectBuffer, DrawArraysIndirectCommand.Stride, ref pts, BufferUsageHint.DynamicDraw);

            _loaded = true;
            _gl.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();
			_program.BindUpdate(_counter++);
			GL.DispatchCompute(_triangleCount, 1, 1);

            //GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);

            _vfShader.Use();
            GL.BindVertexArray(_program.VertexBuffer.VbaId);
            GL.DrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero);

            _gl.SwapBuffers();
        }
		
        private void glControl1_Resize(object sender, EventArgs e)
        {
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
		        data3.Add(Color.FromArgb(1, rnd.Next(128, 255), rnd.Next(1, 255), (int)(i / 100f * 255)));
	        }
	        seriesBuffer.AddSeries(data1);
	        seriesBuffer.AddSeries(data2);
	        seriesBuffer.AddSeries(data3);
        }

    }
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

