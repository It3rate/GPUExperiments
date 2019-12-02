#version 430

layout(std430, binding = 3) buffer FloatSeriesBuffer {
	uint[256] floatSeriesOffsets;
	uint[256] floatSeriesLengths;
	float floatSeriesValues[];
};

float getIndexRemainder(uint seriesIndex, float t, out uint startIndex, out uint endIndex)
{
	uint seriesLength = floatSeriesLengths[seriesIndex];
	float slotCount = seriesLength - 1.0f;
	float result;
	if (t >= 0) {
		startIndex = seriesLength - 1;
		endIndex = startIndex;
		result = 1.0;
	}
	else
	{
		float indexf;
		result = modf(t * slotCount, indexf);
		startIndex = uint(indexf);
		endIndex = startIndex + 1;
	}
	return result;
}

vec4 getLerp2(uint seriesIndex, float t)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, t, startIndex, endIndex);

	vec2 result;
	uint size = 2;
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
	return result;
}
vec4 getLerp3(uint seriesIndex, float t)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, t, startIndex, endIndex);

	vec3 result;
	uint size = 3;
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
	return result;
}
vec4 getLerp4(uint seriesIndex, float t)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, t, startIndex, endIndex);

	vec4 result;
	uint size = 4;
	uint seriesAddress = floatSeriesOffsets[seriesIndex];
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
	return result;
}


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


