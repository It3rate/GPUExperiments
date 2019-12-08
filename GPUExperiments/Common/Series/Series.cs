using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUExperiments.Common.Series
{
	public enum SeriesType : byte
	{
		FloatSeries,
		IntSeries,
	}
    [Flags]
	public enum SampleStyle : byte
	{
		IsBaked,
		ShouldInterpolate,
		Reverse,
    }

    public struct Series
    {
        public ushort Id;
	    public SeriesType Type;
	    public SampleStyle SampleStyle;
        public ushort VectorSize;
	    public uint Count;
    }

    public abstract class SeriesData<T> where T : struct
    {
	    public ushort Id { get; } = 0;
	    public uint Capacity { get; protected set; } = 1;
	    public int Length => _data.Length;
	    public virtual T[] Data { get; }

	    protected Document _document { get; }
	    protected T[] _data;

        //private List<ushort> _refs { get; } = new List<ushort>();
        //private bool _dataChanged;
        //private bool _dataLengthChanged;
    }

    public class FloatSeriesData : SeriesData<float>
    {
	    public override float[] Data => (Id == 0) ? _data : _document.FloatDataStore[Id];
    }
    public class IntSeriesData : SeriesData<int>
    {
	    public override int[] Data => (Id == 0) ? _data : _document.IntDataStore[Id];
    }
}
