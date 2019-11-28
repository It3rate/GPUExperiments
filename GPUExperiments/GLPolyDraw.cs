using System;
using System.Collections.Generic;
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
	struct DrawArraysIndirectCommand
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

		public IntPtr Size => (IntPtr)(4 * sizeof(uint));
    }

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
	        GL.ClearColor(0.2f, 0.0f, 0.0f, 1f);
	        GL.Enable(EnableCap.DepthTest);

	        _vfShader = new VertexFragmentShader("Shaders/polyShader.vert", "Shaders/polyShader.frag");
	        _computeShader = new ComputeShader("Shaders/polyShader.comp");

	        float[] data =
	        {
		        -0.5f, -0.5f,
		        +0.5f, -0.5f,
		        +0.0f, +0.5f
	        };

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            _vba = GL.GenVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BindVertexArray(_vba);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);

	        _indirect = GL.GenBuffer();
	        GL.BindBuffer(BufferTarget.DrawIndirectBuffer, _indirect);
	        var pts = new DrawArraysIndirectCommand(3, 1);
            GL.BufferData(BufferTarget.DrawIndirectBuffer, pts.Size, ref pts, BufferUsageHint.StaticDraw);
            //GL.BindBuffer(BufferTarget.DrawIndirectBuffer, 0);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, _vbo); // Buffer Binding 1

            _loaded = true;
	        _gl.Invalidate();
        }

        private int _indirect;
        private int _vbo;
        private int _vba;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
	        if (!_loaded) return;
	        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

	        _computeShader.Use();
	        GL.DispatchCompute(256, 256, 1);

	        _vfShader.Use();
	        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
	        GL.BindVertexArray(_vba);
            GL.DrawArraysIndirect(PrimitiveType.Triangles, IntPtr.Zero);

	        _gl.SwapBuffers();
        }
        private void glControl1_Resize(object sender, EventArgs e)
        {
        }
    }
}
