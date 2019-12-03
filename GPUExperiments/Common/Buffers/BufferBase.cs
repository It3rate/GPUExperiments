using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common.Buffers
{
	public interface IBuffer
	{
		int Id { get; }
		BufferSlots BufferIndex { get; }

		BufferTarget BufferTarget { get; set; }
		BufferRangeTarget BufferRangeTarget { get; set; }
		BufferUsageHint BufferUsageHint { get; set; }

		bool IsBound { get; }
		int Size { get; }
		int ByteSize { get; }

		void BindData();
        int BindBuffer();
	}


    public abstract class BufferBase : IBuffer
    {
	    public int Id { get; private set; }
	    public abstract BufferSlots BufferIndex { get; }

	    public virtual BufferTarget BufferTarget { get; set; } = BufferTarget.UniformBuffer;
        public virtual BufferRangeTarget BufferRangeTarget { get; set; } = BufferRangeTarget.ShaderStorageBuffer;
        public virtual BufferUsageHint BufferUsageHint { get; set; } = BufferUsageHint.DynamicRead;

	    public bool IsBound{ get; private set; }
        public abstract int Size { get; }
        public abstract int ByteSize { get; }

        protected BufferBase()
        {
	        Id = GL.GenBuffer();
        }

        public abstract void BindData();

        public virtual int BindBuffer()
        {
	        GL.BindBuffer(BufferTarget, Id);
	        BindData();
	        GL.BindBufferBase(BufferRangeTarget, (int)BufferIndex, Id);
	        IsBound = true;
	        return Id;
        }

    }
}
