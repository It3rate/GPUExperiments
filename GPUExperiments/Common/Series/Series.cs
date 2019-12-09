using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static uint SeriesIdCounter = 1;

        public ushort Id;
	    public SeriesType Type;
	    public SampleStyle SampleStyle;
        public ushort VectorSize;
	    public uint Count;
    }

    public abstract class SeriesData<T> where T : struct
    {
        protected static uint NextSeriesId => Series.SeriesIdCounter++;

        public uint Id { get; private set; }
	    public uint Capacity { get; protected set; } = 0;
	    public int Length => LocalData.Length;
	    public abstract T[] Data { get; }

	    protected Document Document { get; }
	    protected T[] LocalData;

        //private List<ushort> _refs { get; } = new List<ushort>();
        //private bool _dataChanged;
        //private bool _dataLengthChanged;

        protected SeriesData(T[] localData)
        {
            Document = Document.ActiveInstance;
            Id = 0;
            LocalData = localData;
            Debug.Assert(localData != null);
        }

        protected SeriesData(uint id)
        {
            Document = Document.ActiveInstance;
            Id = id;
            Debug.Assert(id != 0);
        }
    }

    public class FloatSeriesData : SeriesData<float>
    {
	    public override float[] Data => (Id == 0) ? LocalData : Document.FloatDataStore[Id];

        public FloatSeriesData(float[] localData) : base(localData) { }
        public FloatSeriesData(uint id) : base(id) { }
    }
    public class IntSeriesData : SeriesData<int>
    {
	    public override int[] Data => (Id == 0) ? LocalData : Document.IntDataStore[Id];

        public IntSeriesData(int[] localData) : base(localData) { }
        public IntSeriesData(uint id) : base(id) { }
    }
}
