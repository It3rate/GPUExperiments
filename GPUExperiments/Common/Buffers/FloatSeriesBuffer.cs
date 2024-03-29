﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;

namespace GPUExperiments.Common.Buffers
{
	public class FloatSeriesBuffer : SeriesBufferBase<float>
    {
	    public override BufferSlots BufferIndex => BufferSlots.FloatSeries;

        public FloatSeriesBuffer()
		{
			Pointers = new List<DataPointer>();
			SeriesValues = new List<float[]>();
		}

        public override void AddSeries(float[] values, int vectorSize)
		{
			DataPointer p = new DataPointer(0, (uint) vectorSize, (uint) Size, (uint) values.Length);
			Pointers.Add(p);
			SeriesValues.Add(values);
		}
        public override void AddSeries(List<float[]> values, int vectorSize)
		{
			foreach (var value in values)
			{
				var floats = value.ToArray();
				AddSeries(floats, vectorSize);
			}
		}
        public override void AddSeries(List<Color> colors)
		{
			var floats = RGBAToFloats(colors);
			AddSeries(floats, 4);
		}
        public void AddSeries(List<Vector2> values)
		{
			var floats = VectorToFloats(values);
			AddSeries(floats, 2);
		}
        public void AddSeries(List<Vector3> values)
		{
			var floats = VectorToFloats(values);
			AddSeries(floats, 3);
		}
        public void AddSeries(List<Vector4> values)
		{
			var floats = VectorToFloats(values);
			AddSeries(floats, 4);
		}

		public override float[] FlattenedValues
		{
			get
			{
				int len = Size;
				var result = new float[len];
				int index = 0;
				foreach (var floatSeriesValue in SeriesValues)
				{
					foreach (var f in floatSeriesValue)
					{
						result[index++] = f;
					}
				}

				return result;
			}
		}

        public override int Size
		{
			get
			{
				int len = 0;
				foreach (var floatSeriesValue in SeriesValues)
				{
					len += floatSeriesValue.Length;
				}

				return len;
			}
		}
        public override int ByteSize => Size * sizeof(float);

		public float[] VectorToFloats(List<Vector2> values)
		{
			var result = new float[values.Count * 2];
			int index = 0;
			foreach (var value in values)
			{
				result[index++] = value.X / 255f;
				result[index++] = value.Y / 255f;
			}

			return result;
		}
        public float[] VectorToFloats(List<Vector3> values)
		{
			var result = new float[values.Count * 3];
			int index = 0;
			foreach (var value in values)
			{
				result[index++] = value.X / 255f;
				result[index++] = value.Y / 255f;
				result[index++] = value.Z / 255f;
			}

			return result;
		}
        public float[] VectorToFloats(List<Vector4> values)
		{
			var result = new float[values.Count * 4];
			int index = 0;
			foreach (var value in values)
			{
				result[index++] = value.X / 255f;
				result[index++] = value.Y / 255f;
				result[index++] = value.Z / 255f;
				result[index++] = value.W / 255f;
			}

			return result;
		}
        public float[] RGBAToFloats(List<Color> colors)
		{
			var result = new float[colors.Count * 4];
			int index = 0;
			foreach (var color in colors)
			{
				result[index++] = color.R / 255f;
				result[index++] = color.G / 255f;
				result[index++] = color.B / 255f;
				result[index++] = color.A / 255f;
			}

			return result;
		}
        
	}
}
