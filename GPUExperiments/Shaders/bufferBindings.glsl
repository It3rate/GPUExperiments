#version 430

#define BINDINGS

struct PolyVertex {
	vec4  location;
	vec4  color;
};

layout(std430, binding = 1) buffer VertexBuffer {
	PolyVertex verts[];
};

layout(std430, binding = 2) buffer ProgramState {
	uint currentTicks;
	uint previousTicks;
	uint totalElementCount;
	uint dirtyElementCount;
	uint floatSeriesStartIndex;
	uint intSeriesStartIndex;
	uint activeElementIds[];
};

struct DataPointer {
	uint type;
	uint vecSize;
	uint startAddress;
	uint byteLength;
};

layout(std430, binding = 3) buffer DataPointers {
	DataPointer dataPointers[];
};

layout(std430, binding = 4) buffer FloatSeriesBuffer {
	float floatSeriesValues[];
};

layout(std430, binding = 5) buffer intSeriesBuffer {
	uint[256] intSeriesOffsets;
	uint[256] intSeriesLengths;
	int intSeriesValues[];
};

