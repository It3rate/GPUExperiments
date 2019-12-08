using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUExperiments.Common.Series
{

	public class DataStore<T>
	{
		public List<T[]> Data { get; } = new List<T[]>(0xFFF);
		
		public  T[] this[int index]
		{
			get { return Data[index]; }
		}
	}
	public class FloatDataStore : DataStore<float>
	{
	}
	public class IntDataStore : DataStore<int>
	{
	}
}
