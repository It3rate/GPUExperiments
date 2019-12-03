using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common.Buffers
{
    public class VertexBuffer : BufferBase
    {
	    public int VbaId { get; private set; } 

        public override BufferSlots BufferIndex => BufferSlots.Vertexes;
        public uint Capacity => Math.Max(_setCapacity, (uint)Vertexes.Count);

        public override BufferTarget BufferTarget { get; set; } = BufferTarget.ArrayBuffer;
        public override BufferUsageHint BufferUsageHint { get; set; } = BufferUsageHint.StaticDraw;

        private uint _setCapacity;

	    public List<BasicVertex> Vertexes { get; } = new List<BasicVertex>();

	    public VertexBuffer(uint capacity = 0)
	    {
		    _setCapacity = capacity;
		    VbaId = GL.GenVertexArray();
        }

	    public void AdjustCapacity(uint capacity)
	    {
		    var orgCapacity = _setCapacity;
		    _setCapacity = capacity;
		    if (IsBound && capacity > Vertexes.Count && orgCapacity != _setCapacity)
		    {
			    BindData();
		    }
	    }

        public override void BindData()
        {
	        GL.BindBuffer(BufferTarget, Id);
            if (Vertexes.Count > 0)
		    {
			    GL.BufferData(BufferTarget.ArrayBuffer, ByteSize, Vertexes.ToArray(), BufferUsageHint);
		    }
		    else
		    {
			    GL.BufferData(BufferTarget.ArrayBuffer, ByteSize, IntPtr.Zero, BufferUsageHint);
            }
            GL.BindBuffer(BufferTarget, 0);
        }

        public override int BindBuffer()
        {
	        base.BindBuffer();
			
	        GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
	        GL.BindVertexArray(VbaId);

	        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, BasicVertex.ByteSize, BasicVertex.LocationOffset);
	        GL.EnableVertexAttribArray(0);
	        GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, BasicVertex.ByteSize, BasicVertex.ColorOffset);
	        GL.EnableVertexAttribArray(1);
	        GL.BindVertexArray(0);

	        return Id;
        }

        public override int Size => (int)Capacity;
	    public override int ByteSize => Size * BasicVertex.ByteSize;
    }

    public struct BasicVertex
    {
	    public Vector4 Location;
	    public Vector4 Color;
	    public BasicVertex(Vector4 location, Vector4 color)
	    {
		    Location = location;
		    Color = color;
	    }
	    public BasicVertex(float x, float y, Color color)
	    {
		    Location = new Vector4(x, y, 0, 1f);
		    Color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
	    }
	    public static int LocationOffset => 0;
	    public static int ColorOffset => (int)(sizeof(float) * 4);
	    public static int Size => 8;
	    public static int ByteSize => sizeof(float) * Size;
    }
}
