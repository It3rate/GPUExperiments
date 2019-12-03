using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public class DataPointerBuffer
    {
        private List<IPartialSeries> SeriesBuffers { get; } = new List<IPartialSeries>();

        public DataPointerBuffer(params IPartialSeries[] seriesBuffers)
        {
            SeriesBuffers.AddRange(seriesBuffers);
        }

        public static DataPointer[] ConcatPointers(out uint[] startIndexes, params IPartialSeries[] seriesBuffers)
        {
            List<DataPointer> result = new List<DataPointer>();
            startIndexes = new uint[seriesBuffers.Length];
            uint index = 0;
            for (int i = 0; i < seriesBuffers.Length; i++)
            {
                var sb = seriesBuffers[i];
                result.AddRange(sb.Pointers);
                startIndexes[i] = index;
                index += (uint)(sb.Pointers.Count * DataPointer.Size);
            }
            return result.ToArray();
        }

        public int BindSeriesBuffer(int bufferBindIndex, out uint[] startIndexes)
        {
            var result = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, result);
            var values = ConcatPointers(out startIndexes, SeriesBuffers.ToArray());
            GL.BufferData(BufferTarget.UniformBuffer, values.Length * DataPointer.ByteSize, values, BufferUsageHint.DynamicRead);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, bufferBindIndex, result);
            return result;
        }
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
