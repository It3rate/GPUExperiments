using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
	public interface IBuffer
	{
		int Id { get; }
		int BufferIndex { get; }

		BufferTarget BufferTarget { get; set; }
		BufferRangeTarget BufferRangeTarget { get; set; }
		BufferUsageHint BufferUsageHint { get; set; }
		
		int Size { get; }
		int ByteSize { get; }

		void BindData();
        int BindBuffer();
	}


    public abstract class BufferBase : IBuffer
    {
	    public int Id { get; private set; }
	    public abstract int BufferIndex { get; }

	    public BufferTarget BufferTarget { get; set; } = BufferTarget.UniformBuffer;
	    public BufferRangeTarget BufferRangeTarget { get; set; } = BufferRangeTarget.ShaderStorageBuffer;
	    public BufferUsageHint BufferUsageHint { get; set; } = BufferUsageHint.DynamicRead;
		
        public abstract int Size { get; }
        public abstract int ByteSize { get; }

        public abstract void BindData();

        public int BindBuffer()
        {
	        Id = GL.GenBuffer();
	        GL.BindBuffer(BufferTarget.UniformBuffer, Id);
	        BindData();
	        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
	        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, BufferIndex, Id);
	        return Id;
        }

    }
}
