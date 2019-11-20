using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Timer = System.Windows.Forms.Timer;

namespace GPUExperiments
{
    public class GLView
    {
	    private GLControl _gl;
		Timer _timer;
	    private static readonly Random Random = new Random();

	    private readonly float[] _data =
	    {
		    // Vertices          Colors
		    -0.5f, -0.5f, -0.5f, 1f, 0f, 0f,
		    +0.5f, -0.5f, -0.5f, 0f, 1f, 0f,
		    +0.5f, -0.5f, +0.5f, 0f, 0f, 1f,
		    -0.5f, -0.5f, +0.5f, 0f, 1f, 1f,
		    +0.0f, +0.5f, +0.0f, 1f, 1f, 0f
	    };

	    private readonly uint[] _indices =
	    {
		    // Base
		    0, 2, 1,
		    0, 2, 3,
            
		    // Sides
		    4, 0, 1,
		    4, 1, 2,
		    4, 2, 3,
		    4, 3, 0
	    };

	    // We store the loaded state in a boolean to prevent GL-specific instructions
	    // being called before the GL context is created
	    private bool _loaded;

	    private int _vertexBufferObject;
	    private int _elementBufferObject;
	    private int _vertexArrayObject;

	    private Shader _shader;

	    // We use two TrackBars to rotate the object.
	    // These two fields store the rotation in radians along the X and Y axis.
	    private float _rotationX;
	    private float _rotationY;

        public GLView(GLControl control)
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

	    private void OnTimerOnTick(object sender, EventArgs args)
	    {
		    _rotationX += 0.01f;
		    _rotationY += 0.03f;
            UpdateViewMatrix();
			_gl.Invalidate();
	    }

	    private void glControl1_Load(object sender, EventArgs e)
	    {
		    GL.ClearColor(0f, 0f, 0f, 1f);
		    GL.Enable(EnableCap.DepthTest);

		    _vertexBufferObject = GL.GenBuffer();
		    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
		    GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * sizeof(float), _data, BufferUsageHint.DynamicDraw);

		    _elementBufferObject = GL.GenBuffer();
		    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
		    GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

		    _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
		    _shader.Use();

		    _vertexArrayObject = GL.GenVertexArray();
		    GL.BindVertexArray(_vertexArrayObject);

		    // Because there's now 6 floats (vertex + color data) between the start of the first vertex
		    // and the start of the second we specify a stride of 6 * sizeof(float)
		    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
		    GL.EnableVertexAttribArray(0);

		    // Here we specify an offset of 3 * sizeof(float) as we want to point to the first color
		    // which is after the first coordinate (x, y, z: 3 floats)
		    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
		    GL.EnableVertexAttribArray(1);

		    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
		    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

		    UpdateViewMatrix();
		    _loaded = true;

		    // We have to force a draw or the window will stay blank until we invalidate the control somewhere else.
		    _gl.Invalidate();
        }

	    private void glControl1_Paint(object sender, PaintEventArgs e)
	    {

		    if (!_loaded)
			    return;
			UpdateViewMatrix();

		    // Clears the control using the background color
		    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		    _shader.Use();

		    // Draws the object
		    GL.BindVertexArray(_vertexArrayObject);
		    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

		    // Swaps front frame with back frame
		    _gl.SwapBuffers();
        }

	    private void glControl1_Resize(object sender, EventArgs e)
	    {
		    // As the object is firstly created with an empty constructor and all the properties are set afterwards
		    // it's likely that this event gets fired once or twice before OpenGL initialization (Load event)
		    // hence it's important to include the loaded check.
		    if (!_loaded)
			    return;

		    GL.Viewport(0, 0, _gl.Width, _gl.Height);

            // We invalidate the control to apply the viewport changes.
            // Invalidating the control forces the Paint event to be fired.
            // Without invalidating no 

	    }
	    private void UpdateViewMatrix()
	    {
		    if (!_loaded)
			    return;

		    _shader.Use();

		    // We update the view matrix with the new rotation data
		    var view = Matrix4.CreateRotationX(_rotationX);
		    view *= Matrix4.CreateRotationY(_rotationY);
		    _shader.SetMatrix4("view", view);
	    }
    }
}
