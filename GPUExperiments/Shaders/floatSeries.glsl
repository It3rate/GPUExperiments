#version 430

layout(std430, binding = 3) buffer FloatSeriesBuffer {
	uint[256] floatSeriesOffsets;
	float floatSeriesValues[];
};


float floatAt(uint seriesIndex, uint index)
{
	int seriesAddress = floatSeriesOffset[seriesIndex];
	return floatSeriesOffset[seriesAddress + index];
}
vec2 vec2At(uint seriesIndex, uint index)
{
	int seriesAddress = floatSeriesOffset[seriesIndex];
	int startAddress = seriesAddress + index * 2;
	return vec2(
		floatSeriesOffset[startAddress + 0],
		floatSeriesOffset[startAddress + 1]);
}
vec3 vec3At(uint seriesIndex, uint index)
{
	int seriesAddress = floatSeriesOffset[seriesIndex];
	int startAddress = seriesAddress + index * 3;
	return vec3(
		floatSeriesOffset[startAddress + 0],
		floatSeriesOffset[startAddress + 1],
		floatSeriesOffset[startAddress + 2]);
}
vec4 vec4At(uint seriesIndex, uint index)
{
	int seriesAddress = floatSeriesOffset[seriesIndex];
	int startAddress = seriesAddress + index * 4;
	return vec4(
		floatSeriesOffset[startAddress + 0],
		floatSeriesOffset[startAddress + 1],
		floatSeriesOffset[startAddress + 2],
		floatSeriesOffset[startAddress + 3]);
}