﻿using System;
using System.Windows.Forms;
using GPUExperiments.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments
{
    public class GLOneTriangle
    {
        private VertexFragmentShader _vfShader;
        private ComputeShader _computeShader;
        private GLControl _gl;
        private bool _loaded;

        public GLOneTriangle(GLControl control)
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

            _vfShader = new VertexFragmentShader("Shaders/basic.vert", "Shaders/basic.frag");
            _computeShader = new ComputeShader("Shaders/basic.comp");

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

            _loaded = true;
            _gl.Invalidate();
        }

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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            _gl.SwapBuffers();
        }
        private void glControl1_Resize(object sender, EventArgs e)
        {
        }
    }
}
