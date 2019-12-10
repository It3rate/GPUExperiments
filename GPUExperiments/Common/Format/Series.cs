using System;
using System.Diagnostics;

namespace GPUExperiments.Common.Format
{
	public enum SeriesType : byte
	{
		FloatSeries,
		IntSeries,
	}

    public struct SeriesUtils
    {
        public static uint SeriesIdCounter = 1;
		
    }

	/// <summary>
    /// A series data block that can be int or float based, and stored in the document (format) or in place (working).
    /// </summary>
    /// <typeparam name="T">Float or Int.</typeparam>
    public abstract class SeriesData<T> where T : struct
    {
        protected static uint NextSeriesId => SeriesUtils.SeriesIdCounter++;

        public uint Id { get; private set; }
        public abstract SeriesType SeriesType { get; }
        public ushort VectorSize;

	    public int Length => Data.Length;
	    public T[] Data => (Id == 0) ? LocalData : BlockData[Id];

        protected Document Document { get; }
	    protected T[] LocalData;
	    protected abstract DataBlocks<T> BlockData { get; }

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
	    public override SeriesType SeriesType => SeriesType.FloatSeries;
	    protected override DataBlocks<float> BlockData => Document.FloatBlocks;

	    public FloatSeriesData(float[] localData) : base(localData) { }
        public FloatSeriesData(uint id) : base(id) { }
    }

    public class IntSeriesData : SeriesData<int>
    {
	    public override SeriesType SeriesType => SeriesType.IntSeries;
        protected override DataBlocks<int> BlockData => Document.IntBlocks;

        public IntSeriesData(int[] localData) : base(localData) { }
        public IntSeriesData(uint id) : base(id) { }
    }
}
