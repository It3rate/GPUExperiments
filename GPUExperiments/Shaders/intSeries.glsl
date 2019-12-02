#version 430

layout(std430, binding = 5) buffer intSeriesBuffer {
	uint[256] intSeriesOffsets;
	uint[256] intSeriesLengths;
	int intSeriesValues[];
};


int intAt(uint seriesIndex, uint index)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	return intSeriesValues[seriesAddress + index];
}

void ivecAt(uint seriesIndex, uint index, inout ivec2 result)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 2;
	result.x = intSeriesValues[startAddress + 0];
	result.y = intSeriesValues[startAddress + 1];
}
void ivecAt(uint seriesIndex, uint index, inout ivec3 result)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 3;
	result.x = intSeriesValues[startAddress + 0];
	result.y = intSeriesValues[startAddress + 1];
	result.z = intSeriesValues[startAddress + 2];
}
void ivecAt(uint seriesIndex, uint index, inout ivec4 result)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 4;
	result.x = intSeriesValues[startAddress + 0];
	result.y = intSeriesValues[startAddress + 1];
	result.z = intSeriesValues[startAddress + 2];
	result.w = intSeriesValues[startAddress + 3];
}


