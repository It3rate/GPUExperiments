using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public abstract class SeriesBufferBase<T>
    {

	    public List<DataPointer> Pointers { get; protected set; }
	    public List<T[]> SeriesValues { get; protected set; }

	    public abstract void AddSeries(T[] values, int vectorSize);
        public abstract void AddSeries(List<T[]> values, int vectorSize);
        public abstract void AddSeries(List<Color> colors);

        public abstract int ValuesSize { get; }
	    public abstract int ValuesByteSize { get; }
        public abstract int PointersSize { get; }
        public abstract int PointersByteSize { get; }
        public abstract T[] FlattenedValues { get; }



        public abstract int BindSeriesBuffer(int bufferBindIndex,
		    BufferTarget bufferTarget = BufferTarget.UniformBuffer,
		    BufferRangeTarget bufferRangeTarget = BufferRangeTarget.ShaderStorageBuffer,
		    BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicRead);
    }
}
