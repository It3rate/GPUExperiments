using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public class DataPointerBuffer : BufferBase
    {
	    public override int BufferIndex => (int)BufferSlots.DataPointers;

        private List<IPartialSeries> SeriesBuffers { get; } = new List<IPartialSeries>();

        public DataPointerBuffer(params IPartialSeries[] seriesBuffers)
        {
            SeriesBuffers.AddRange(seriesBuffers);
        }

        public void AddSeries(IPartialSeries seriesBuffer)
        {
			SeriesBuffers.Add(seriesBuffer);
        }

        public static DataPointer[] ConcatPointers(params IPartialSeries[] seriesBuffers)
        {
            List<DataPointer> result = new List<DataPointer>();
            foreach (var seriesBuffer in seriesBuffers)
            {
	            result.AddRange(seriesBuffer.Pointers);
            }
            return result.ToArray();
        }

        public override void BindData()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, Id);
            var values = ConcatPointers(SeriesBuffers.ToArray());
            GL.BufferData(BufferTarget.UniformBuffer, values.Length * DataPointer.ByteSize, values, BufferUsageHint.DynamicRead);
        }
        public override int Size => ConcatPointers(SeriesBuffers.ToArray()).Length;
        public override int ByteSize => Size * DataPointer.ByteSize;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DataPointer
    {
        [FieldOffset(0)]
        public uint Type;
        [FieldOffset(4)]
        public uint VecSize;
        [FieldOffset(8)]
        public uint StartAddress;
        [FieldOffset(12)]
        public uint ByteLength;

        public DataPointer(uint type, uint vecSize, uint startAddress, uint byteLength)
        {
            Type = type;
            VecSize = vecSize;
            StartAddress = startAddress;
            ByteLength = byteLength;
        }

        public uint[] FlattenedPointer => new []{Type, VecSize, StartAddress, ByteLength};
        public static int Size => 4;
        public static int ByteSize => sizeof(uint) * Size;
    }

}
