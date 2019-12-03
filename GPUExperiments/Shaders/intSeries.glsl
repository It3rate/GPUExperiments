#version 430

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


