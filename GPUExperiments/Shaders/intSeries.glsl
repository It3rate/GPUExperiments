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

ivec2 ivec2At(uint seriesIndex, uint index)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 2;
	return ivec2(
		intSeriesValues[startAddress + 0],
		intSeriesValues[startAddress + 1]);
}
ivec3 ivec3At(uint seriesIndex, uint index)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 3;
	return ivec3(
		intSeriesValues[startAddress + 0],
		intSeriesValues[startAddress + 1],
		intSeriesValues[startAddress + 2]);
}
ivec4 ivec4At(uint seriesIndex, uint index)
{
	uint seriesAddress = intSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 4;
	return ivec4(
		intSeriesValues[startAddress + 0],
		intSeriesValues[startAddress + 1],
		intSeriesValues[startAddress + 2],
		intSeriesValues[startAddress + 3]);
}


