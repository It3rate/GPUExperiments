#version 430

layout(std430, binding = 3) buffer FloatSeriesBuffer {
	uint[256] floatSeriesOffsets;
	float floatSeriesValues[];
};


float floatAt(uint seriesIndex, uint index)
{
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	return floatSeriesValues[seriesAddress + index];
}

vec2 vec2At(uint seriesIndex, uint index)
{
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 2;
	return vec2(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1]);
}
vec3 vec3At(uint seriesIndex, uint index)
{
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 3;
	return vec3(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1],
		floatSeriesValues[startAddress + 2]);
}
vec4 vec4At(uint seriesIndex, uint index)
{
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startAddress = seriesAddress + index * 4;
	return vec4(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1],
		floatSeriesValues[startAddress + 2],
		floatSeriesValues[startAddress + 3]);
}


