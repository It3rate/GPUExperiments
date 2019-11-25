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
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GPUExperiments
{
    public class GLHexGrid
    {
        private int _textureId;
        private VertexFragmentShader _vfShader;
        private ComputeShader _computeShader;
        private int _vfShaderID;
        private int _computeShaderID;
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
            _vfShader = new VertexFragmentShader("Shaders/shaderSimple.vert", "Shaders/shaderSimple.frag");
            _vfShaderID = _vfShader.ProgramId;
            _computeShader = new ComputeShader("Shaders/shaderSimple.comp");
            _computeShaderID = _computeShader.ProgramId;
            //_textureId = GenerateDestTex();
            _textureId = CreateTexture(TextureUnit.Texture0, Resources.glTest5);
            //_vfShaderID = SetupRenderProgram(_textureId);
            //_computeShaderID = SetupComputeProgram(_textureId);
			
            //GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 0, out int xc);

            _loaded = true;
            _gl.Invalidate();
        }

        public static int CreateTexture(TextureUnit textureUnit, Bitmap bmp)
        {
	        int result = GL.GenTexture();

	        GL.ActiveTexture(textureUnit);
	        GL.BindTexture(TextureTarget.Texture2D, result);
	        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
	        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

	        BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
		        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

	        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
		        bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

	        int unitIndex = textureUnit - TextureUnit.Texture0;
	        GL.BindImageTexture(unitIndex, result, 0, false, 0,
		        TextureAccess.ReadWrite, SizedInternalFormat.Rgba8);

	        return result;
        }

        private int GenerateDestTex()
        {
	        // We create a single float channel 512^2 texture
	        int result = GL.GenTexture();

	        GL.ActiveTexture(TextureUnit.Texture0);
	        GL.BindTexture(TextureTarget.Texture2D, result);
	        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
	        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

	        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
		        256, 256, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
			
	        GL.BindImageTexture(0, result, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba8);
	        return result;
        }
        private int SetupComputeProgram(int texHandle)
        {
	        // Creating the compute shader, and the program object containing the shader
	        int progHandle = GL.CreateProgram();
	        int cs = GL.CreateShader(ShaderType.ComputeShader);

	        // In order to write to a texture, we have to introduce it as image2D.
	        // local_size_x/y/z layout variables define the work group size.
	        // gl_GlobalInvocationID is a uvec3 variable giving the global ID of the thread,
	        // gl_LocalInvocationID is the local index within the work group, and
	        // gl_WorkGroupID is the work group's index
	        string csSrc =
		        "#version 430\n" +
		        "uniform float roll; " +
		        "uniform writeonly image2D destTex; " +
		        "layout (local_size_x = 16, local_size_y = 16) in; " +
		        "void main() { " +
		        "ivec2 storePos = ivec2(gl_GlobalInvocationID.xy); " +
		        "float localCoef = length(vec2(ivec2(gl_LocalInvocationID.xy)-8)/8.0); " +
		        "float globalCoef = sin(float(gl_WorkGroupID.x+gl_WorkGroupID.y)*0.1 + roll)*0.5; " +
		        "imageStore(destTex, storePos, vec4(1.0-globalCoef*localCoef, 0.0, 0.0, 0.0)); " +
		        "} ";

	        GL.ShaderSource(cs, csSrc);
	        GL.CompileShader(cs);
	        int rvalue;
	        GL.GetShader(cs, ShaderParameter.CompileStatus, out rvalue);
	        if (rvalue != (int)All.True)
	        {
		        Console.WriteLine(GL.GetShaderInfoLog(cs));
	        }
	        GL.AttachShader(progHandle, cs);

	        GL.LinkProgram(progHandle);
	        GL.GetProgram(progHandle, GetProgramParameterName.LinkStatus, out rvalue);
	        if (rvalue != (int)All.True)
	        {
		        Console.WriteLine(GL.GetProgramInfoLog(progHandle));
	        }

	        GL.UseProgram(progHandle);

	        GL.Uniform1(GL.GetUniformLocation(progHandle, "destTex"), 0);

	        //checkErrors("Compute shader");
	        return progHandle;
        }
        private int SetupRenderProgram(int texHandle)
        {
            int progHandle = GL.CreateProgram();
            int vp = GL.CreateShader(ShaderType.VertexShader);
            int fp = GL.CreateShader(ShaderType.FragmentShader);

            string vpSrc =
            "#version 430\n" +
            "in vec2 pos; " +
            "out vec2 texCoord; " +
            "void main() { " +
                "texCoord = pos*0.5f + 0.5f; " +
                "gl_Position = vec4(pos.x, pos.y, 0.0, 1.0); " +
            "} ";


            string fpSrc =
                "#version 430\n" +
                "uniform sampler2D srcTex; " +
                "in vec2 texCoord; " +
                "out vec4 color; " +
                "void main() { " +
                "float c = texture(srcTex, texCoord).x; " +
                "color = vec4(c, 1.0, 1.0, 1.0); " +
                "} ";


            GL.ShaderSource(vp, vpSrc);
            GL.ShaderSource(fp, fpSrc);

            GL.CompileShader(vp);
            int rvalue;
            GL.GetShader(vp, ShaderParameter.CompileStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in compiling vp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetShaderInfoLog(vp));
            }
            GL.AttachShader(progHandle, vp);

            GL.CompileShader(fp);
            GL.GetShader(fp, ShaderParameter.CompileStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in compiling fp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetShaderInfoLog(fp));
            }
            GL.AttachShader(progHandle, fp);

            GL.BindFragDataLocation(progHandle, 0, "color");
            GL.LinkProgram(progHandle);

            GL.GetProgram(progHandle, GetProgramParameterName.LinkStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                Console.WriteLine("Error in linking sp");
                Console.WriteLine((All)rvalue);
                Console.WriteLine(GL.GetProgramInfoLog(progHandle));
            }

            GL.UseProgram(progHandle);
            GL.Uniform1(GL.GetUniformLocation(progHandle, "srcTex"), 0);

            int vertArray;
            vertArray = GL.GenVertexArray();
            GL.BindVertexArray(vertArray);

            int posBuf;
            posBuf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            float[] data = {
                -1.0f, -1.0f,
                -1.0f, 1.0f,
                1.0f, -1.0f,
                1.0f, 1.0f
            };
            IntPtr dataSize = (IntPtr)(sizeof(float) * 8);

            GL.BufferData<float>(BufferTarget.ArrayBuffer, dataSize, data, BufferUsageHint.StreamDraw);
            int posPtr = GL.GetAttribLocation(progHandle, "pos");
            GL.VertexAttribPointer(posPtr, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(posPtr);

            //checkErrors("Render shaders");
            return progHandle;
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

        private int frame = 0;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!_loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_computeShader.Use();
            GL.UseProgram(_computeShaderID);
            //GL.BindImageTexture(0, _textureId, 0, false, 0,
	           // TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);

            frame++;
            if (frame == 1024) frame = 0;
            GL.Uniform1(GL.GetUniformLocation(_computeShaderID, "roll"), (float)frame * 0.01f);
            GL.DispatchCompute(256/16,256/16,1);
            //GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            //_vfShader.Use();
            GL.UseProgram(_vfShaderID);
            //GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            //         GL.ActiveTexture(0);
            //GL.BindTexture(TextureTarget.ProxyTexture2D, _textureId);



            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int index = x + y * cols;
                    var view = Matrix4.Identity;
                    // view *= Matrix4.CreateTranslation(-0.5f, 0.5f, 0);
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
