using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPUExperiments.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments
{
    public class GLPolyDraw
    {
	    private int _textureId;
	    private VertexFragmentShader _vfShader;
	    private ComputeShader _computeShader;
        private GLControl _gl;
	    private bool _loaded;

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
	    }

        private void glControl1_Load(object sender, EventArgs e)
        {
	        int maxVertexUniformComponents = GL.GetInteger(GetPName.MaxVertexUniformComponents);
	        int maxTextureBufferSize = GL.GetInteger(GetPName.MaxTextureBufferSize);

            GL.ClearColor(0.2f, 0.0f, 0.0f, 1f);
	        GL.Enable(EnableCap.DepthTest);

	        _vfShader = new VertexFragmentShader("Shaders/polyShader.vert", "Shaders/polyShader.frag");
	        _computeShader = new ComputeShader("Shaders/polyShader.comp");

	        PolyVertex[] data =
	        {
		        new PolyVertex(-0.5f, -0.5f, Color.Green),
		        new PolyVertex(+0.5f, -0.5f, Color.Yellow),
                new PolyVertex(+0.0f, +0.5f, Color.Blue)
            };

	        uint vertexCount = _triangleCount * sideCount * 3;

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (int)vertexCount * PolyVertex.ByteSize, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _vbo);

            _vba = GL.GenVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BindVertexArray(_vba);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, PolyVertex.ByteSize, PolyVertex.LocationOffset);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, PolyVertex.ByteSize, PolyVertex.ColorOffset);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            _indirect = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.DrawIndirectBuffer, _indirect);
            var pts = new DrawArraysIndirectCommand(vertexCount, 1); // vertCount, instCount 
            GL.BufferData(BufferTarget.DrawIndirectBuffer, DrawArraysIndirectCommand.Stride, ref pts, BufferUsageHint.DynamicDraw);
  
            _programState = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, _programState);
            GL.BufferData(BufferTarget.UniformBuffer, _state.ByteSize, _state.Buffer, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, _programState);// Buffer Binding 2

            _loaded = true;
	        _gl.Invalidate();
        }

        uint sideCount = 6;
        private uint _triangleCount = 22* 22;
        private int _indirect;
        private int _vbo;
        private int _vba;
        private int _counter = 0;
        private int _programState;
        private ProgramState _state = new ProgramState(0,0,1,new int[]{0});

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
	        if (!_loaded) return;
	        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _computeShader.Use();
            _state.CurrentTicks = _counter++;
            GL.BindBuffer(BufferTarget.UniformBuffer, _programState);
            GL.BufferData(BufferTarget.UniformBuffer, _state.ByteSize, _state.Buffer, BufferUsageHint.DynamicRead);
            GL.DispatchCompute(_triangleCount, 1, 1);

			GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);

            _vfShader.Use();
	        GL.BindVertexArray(_vba);
            GL.DrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero);

	        _gl.SwapBuffers();
            _gl.Invalidate();
        }
        private void glControl1_Resize(object sender, EventArgs e)
        {
        }
    }

    public struct ProgramState
    {
        public int CurrentTicks;
        public int PreviousTicks;
        public int TotalElementCount;
        public int[] ActiveElementIds;
        public ProgramState(int currentTicks, int previousTicks, int totalElementCount, int[] activeElementIds)
        {
            CurrentTicks = currentTicks;
            PreviousTicks = previousTicks;
            TotalElementCount = totalElementCount;
            ActiveElementIds = activeElementIds ?? new int[]{};
        }
        public int Size => 3 + ActiveElementIds.Length;
        public int ByteSize => sizeof(float) * Size;
        public int[] Buffer
        {
            get
            {
                int[] result = new int[Size];
                result[0] = CurrentTicks;
                result[1] = PreviousTicks;
                result[2] = TotalElementCount;
                for (int i = 0; i < ActiveElementIds.Length; i++)
                {
                    result[3 + i] = ActiveElementIds[i];
                }
                return result;
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

}
