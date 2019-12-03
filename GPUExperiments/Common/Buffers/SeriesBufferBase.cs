using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common.Buffers
{
	public interface IPartialSeries
    {
        List<DataPointer> Pointers { get; }
    }

    public abstract class SeriesBufferBase<T> : BufferBase, IPartialSeries where T : struct
    {
        public List<DataPointer> Pointers { get; protected set; }
	    public List<T[]> SeriesValues { get; protected set; }

	    public abstract void AddSeries(T[] values, int vectorSize);
        public abstract void AddSeries(List<T[]> values, int vectorSize);
        public abstract void AddSeries(List<Color> colors);
		
        public abstract T[] FlattenedValues { get; }

        public override void BindData()
        {
	        GL.BindBuffer(BufferTarget, Id);
	        GL.BufferData(BufferTarget, ByteSize, FlattenedValues, BufferUsageHint);
	        GL.BindBuffer(BufferTarget, 0);
        }
    }
}
