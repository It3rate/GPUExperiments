using System.Collections.Generic;

namespace GPUExperiments.Common.Format
{
    public class DataBlocks<T>
	{
		public Dictionary<uint, T[]> Data { get; } = new Dictionary<uint, T[]>(0xFFF);
        public T[] this[uint index] => Data[index];
    }
	public class FloatBlocks : DataBlocks<float>
	{
	}
	public class IntBlocks : DataBlocks<int>
	{
	}
}
