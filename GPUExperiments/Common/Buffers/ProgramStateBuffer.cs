using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common.Buffers
{
    public class ProgramStateBuffer : BufferBase
    {
        public override BufferSlots BufferIndex => BufferSlots.ProgramState;

        private uint[] _startIndexes;
        private ProgramState _state = new ProgramState();
	    public ProgramState State{get => _state;}

	    public VertexBuffer VertexBuffer { get; } = new VertexBuffer();
        public DataPointerBuffer DataPointers { get; } = new DataPointerBuffer();
        public FloatSeriesBuffer FloatSeries { get; } = new FloatSeriesBuffer();
	    public IntSeriesBuffer IntSeries { get; } = new IntSeriesBuffer();

	    public ProgramStateBuffer()
	    {
		    DataPointers.AddSeries(FloatSeries);
		    DataPointers.AddSeries(IntSeries);
        }

	    public void BindUpdate(uint ticks)
	    {
		    _state.CurrentTicks = ticks;
            _state.FloatSeriesStartIndex = 0;
		    _state.IntSeriesStartIndex = (uint)FloatSeries.Pointers.Count;
			BindData();
        }

        public void BindAll()
        {
	        VertexBuffer.BindBuffer();
	        FloatSeries.BindBuffer();
            IntSeries.BindBuffer();
	        DataPointers.BindBuffer();
            BindBuffer();
        }

        public override void BindData()
        {
		    GL.BindBuffer(BufferTarget.UniformBuffer, Id);
	        GL.BufferData(BufferTarget.UniformBuffer, ByteSize, ref _state, BufferUsageHint.DynamicRead);
	        GL.BindBuffer(BufferTarget, 0);
        }

        public override int Size => _state.Size;
        public override int ByteSize => _state.ByteSize;
    }

    public enum BufferSlots
    {
	    None = 0,
	    Vertexes = 1,
	    ProgramState = 2,
	    DataPointers = 3,
	    FloatSeries = 4,
	    IntSeries = 5,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ProgramState
    {
	    [FieldOffset(0)] public uint CurrentTicks;
	    [FieldOffset(4)] public uint PreviousTicks;
	    [FieldOffset(8)] public uint TotalElementCount;
	    [FieldOffset(12)] public uint DirtyElementCount;
	    [FieldOffset(16)] public uint FloatSeriesStartIndex;
	    [FieldOffset(20)] public uint IntSeriesStartIndex;
	    [FieldOffset(24)] public uint[] ActiveElementIds;
		
        public ProgramState(uint currentTicks,
		    uint previousTicks,
		    uint totalElementCount,
		    uint dirtyElementCount,
		    uint[] activeElementIds = null)
	    {
		    CurrentTicks = currentTicks;
		    PreviousTicks = previousTicks;
		    TotalElementCount = totalElementCount;
		    DirtyElementCount = dirtyElementCount;
		    FloatSeriesStartIndex = 0;
		    IntSeriesStartIndex = 0;
		    ActiveElementIds = activeElementIds ?? new uint[] { };
	    }

	    public int Size => 6 + (ActiveElementIds?.Length ?? 0);
	    public int ByteSize => sizeof(uint) * Size;
    }

}
