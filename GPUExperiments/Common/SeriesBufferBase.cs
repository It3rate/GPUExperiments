using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{

    public interface IPartialSeries
    {
        List<DataPointer> Pointers { get; }
        uint[] FlattenedPointers { get; }
    }

    public abstract class SeriesBufferBase<T> : IPartialSeries where T : struct
    {
		public abstract int BufferIndex { get; }
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

        public uint[] FlattenedPointers
        {
            get
            {
                int len = Pointers.Count * DataPointer.Size;
                var result = new uint[len];
                int index = 0;
                foreach (var p in Pointers)
                {
                    Array.Copy(p.FlattenedPointer, 0, result, index, DataPointer.Size);
                    index += DataPointer.Size;
                }
                return result;
            }
        }

        public int BindSeriesBuffer(
		    BufferTarget bufferTarget = BufferTarget.UniformBuffer,
		    BufferRangeTarget bufferRangeTarget = BufferRangeTarget.ShaderStorageBuffer,
		    BufferUsageHint bufferUsageHint = BufferUsageHint.DynamicRead)
        {
            var result = GL.GenBuffer();
            GL.BindBuffer(bufferTarget, result);
            GL.BufferData(bufferTarget, ValuesByteSize, FlattenedValues, bufferUsageHint);
            GL.BindBuffer(bufferTarget, 0);
            GL.BindBufferBase(bufferRangeTarget, BufferIndex, result);
            return result;
        }
    }
}
