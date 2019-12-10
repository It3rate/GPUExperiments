using System;

namespace GPUExperiments.Common.Format
{
	[Flags]
	public enum SampleMetadata : byte
	{
		IsBaked,
		ShouldInterpolate,
		Reversed,
	}

    public class Sampler
    {
	    public uint Capacity { get; protected set; } = 0;

	    public SampleMetadata SampleMetadata;
    }
}
