#version 430

layout(std430, binding = 3) buffer FloatSeriesBuffer {
	float floatSeriesValues[];
};

struct DataPointer {
	uint type;
	uint vecSize;
	uint startAddress;
	uint byteLength;
};
layout(std430, binding = 4) buffer SeriesPointers {
	DataPointer dataPointers[];
};


float getIndexRemainder(uint seriesIndex, uint vecSize, float t, out uint startIndex, out uint endIndex)
{
	DataPointer dp = dataPointers[seriesIndex];
	uint seriesLength = dp.byteLength / dp.vecSize;
	float slotCount = seriesLength - 1.0f;
	float result;
	if (t >= 1.0) {
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

vec2 getLerp2(uint seriesIndex, float t)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, 2, t, startIndex, endIndex);

	vec2 result;
	uint size = 2;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
	return result;
}
vec3 getLerp3(uint seriesIndex, float t)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, 3, t, startIndex, endIndex);

	vec3 result;
	uint size = 3;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
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
	float rem = getIndexRemainder(seriesIndex, 4, t, startIndex, endIndex);

	//startIndex = 2;
	//endIndex = 3;
	//rem = 0.5f;
	vec4 result;
	uint size = 4;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
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
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	return floatSeriesValues[seriesAddress + index];
}

vec2 vec2At(uint seriesIndex, uint index)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 2;
	return vec2(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1]);
}
vec3 vec3At(uint seriesIndex, uint index)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 3;
	return vec3(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1],
		floatSeriesValues[startAddress + 2]);
}
vec4 vec4At(uint seriesIndex, uint index)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 4;
	return vec4(
		floatSeriesValues[startAddress + 0],
		floatSeriesValues[startAddress + 1],
		floatSeriesValues[startAddress + 2],
		floatSeriesValues[startAddress + 3]);
}


