using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUExperiments.Common.Series
{
    public class DataStore<T>
	{
		public Dictionary<uint, T[]> Data { get; } = new Dictionary<uint, T[]>(0xFFF);
        public T[] this[uint index] => Data[index];
    }
	public class FloatDataStore : DataStore<float>
	{
	}
	public class IntDataStore : DataStore<int>
	{
	}
}
