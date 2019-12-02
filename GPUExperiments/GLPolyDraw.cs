﻿using System;
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
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _vbo);

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

            _programState = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _programState);
            GL.BufferData(BufferTarget.UniformBuffer, _state.ByteSize, ref _state, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _programState); // Buffer Binding 2

            var data1 = new List<Color>(){
                Color.Red, Color.Orange, Color.Yellow, Color.GreenYellow, Color.Green, Color.Blue, Color.BlueViolet, Color.Violet};
            var data2 = new List<Color>(){
                Color.DarkRed, Color.DarkOrange, Color.DarkGoldenrod, Color.DarkGreen, Color.DarkCyan, Color.DarkBlue, Color.DarkViolet, Color.DarkMagenta};
			var data3 = new List<Color>();
			for (int i = 0; i < 100; i++)
			{
				data3.Add(Color.FromArgb(1, rnd.Next(128,255), rnd.Next(1, 255), (int)(i / 100f * 255)));
			}
            seriesData = new FloatSeriesBuffer();
            seriesData.AddSeries(data1);
            seriesData.AddSeries(data2);
            seriesData.AddSeries(data3);

            _floatSeries = seriesData.BindSeriesBuffer(3);
	           // GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.UniformBuffer, _floatSeries);
            //GL.BufferData(BufferTarget.UniformBuffer, seriesData.ValuesByteSize, seriesData.FlattenedValues, BufferUsageHint.DynamicRead);
            //GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 3, _floatSeries); // Buffer Binding3

            _pointers = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _pointers);
            DataPointer[] pData = seriesData.Pointers.ToArray();
            GL.BufferData(BufferTarget.UniformBuffer, seriesData.PointersByteSize, pData, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, _pointers); // Buffer Binding3

            _loaded = true;
            _gl.Invalidate();
        }

        uint sideCount = 6;
        private uint _triangleCount = 22 * 22;
        private int _indirect;
        private int _vbo;
        private int _vba;
        private uint _counter = 0;
        private int _programState;
        private int _floatSeries;
        private FloatSeriesBuffer seriesData;
        private int _pointers;
        private ProgramState _state = new ProgramState(0, 0, 1, new uint[] { 0 });

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();

            _state.CurrentTicks = _counter++;
            _state.FloatSeriesStartIndex = 0;
            _state.IntSeriesStartIndex = (uint)seriesData.PointersSize;
            GL.BindBuffer(BufferTarget.UniformBuffer, _programState);
            GL.BufferData(BufferTarget.UniformBuffer, _state.ByteSize, ref _state, BufferUsageHint.DynamicRead);
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

[StructLayout(LayoutKind.Explicit)]
public struct DataPointer
{
	[FieldOffset(0)]
    public uint Type;
    [FieldOffset(4)]
    public uint VecSize;
    [FieldOffset(8)]
    public uint StartAddress;
    [FieldOffset(12)]
    public uint ByteLength;

    public DataPointer(uint type, uint vecSize, uint startAddress, uint byteLength)
    {
        Type = type;
        VecSize = vecSize;
        StartAddress = startAddress;
        ByteLength = byteLength;
    }
    public static int Size => 4;
    public static int ByteSize => sizeof(float) * Size;
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

[StructLayout(LayoutKind.Explicit)]
public struct ProgramState
{
    [FieldOffset(0)] public uint CurrentTicks;
    [FieldOffset(4)] public uint PreviousTicks;
    [FieldOffset(8)] public uint TotalElementCount;
    [FieldOffset(12)] public uint FloatSeriesStartIndex;
    [FieldOffset(16)] public uint IntSeriesStartIndex;
    [FieldOffset(24)] public uint[] ActiveElementIds;

    public ProgramState(uint currentTicks, uint previousTicks, uint totalElementCount, uint[] activeElementIds)
    {
        CurrentTicks = currentTicks;
        PreviousTicks = previousTicks;
        TotalElementCount = totalElementCount;
        FloatSeriesStartIndex = 0;
        IntSeriesStartIndex = 0;
        ActiveElementIds = new uint[] { 0 };
    }

    public int Size => 6 + ActiveElementIds.Length;
    public int ByteSize => sizeof(uint) * Size;
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

