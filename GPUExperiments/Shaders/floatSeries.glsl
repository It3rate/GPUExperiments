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

void getLerp(uint seriesIndex, float t, inout vec2 result)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, 2, t, startIndex, endIndex);

	uint size = 2;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
}
void getLerp(uint seriesIndex, float t, inout vec3 result)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, 3, t, startIndex, endIndex);

	uint size = 3;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
}
void getLerp(uint seriesIndex, float t, inout vec4 result)
{
	uint startIndex, endIndex;
	float rem = getIndexRemainder(seriesIndex, 4, t, startIndex, endIndex);

	uint size = 4;
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startByte = seriesAddress + startIndex * size;
	uint endByte = seriesAddress + endIndex * size;
	for (int i = 0; i < size; i++)
	{
		result[i] = mix(floatSeriesValues[startByte + i], floatSeriesValues[endByte + i], rem);
	}
}


float floatAt(uint seriesIndex, uint index)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	return floatSeriesValues[seriesAddress + index];
}

void vecAt(uint seriesIndex, uint index, inout vec2 result)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 2;
	result.x = floatSeriesValues[startAddress + 0];
	result.y = floatSeriesValues[startAddress + 1];
}
void vecAt(uint seriesIndex, uint index, inout vec3 result)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 3;

	result.x = floatSeriesValues[startAddress + 0];
	result.y = floatSeriesValues[startAddress + 1];
	result.z = floatSeriesValues[startAddress + 2];
}
void vecAt(uint seriesIndex, uint index, inout vec4 result)
{
	uint seriesAddress = dataPointers[seriesIndex].startAddress;
	uint startAddress = seriesAddress + index * 4;
	result.x = floatSeriesValues[startAddress + 0];
	result.y = floatSeriesValues[startAddress + 1];
	result.z = floatSeriesValues[startAddress + 2];
	result.w = floatSeriesValues[startAddress + 3];
}


