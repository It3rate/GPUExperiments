using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace GPUExperiments.Common
{
	public class FloatSeriesBuffer
	{
		public List<DataPointer> pointers;
		public List<float[]> floatSeriesValues;
		private int itemIndex;

		public FloatSeriesBuffer()
		{
			pointers = new List<DataPointer>();
			floatSeriesValues = new List<float[]>();
			itemIndex = 0;
		}

		public void AddSeries(float[] values, int vectorSize)
		{
			DataPointer p = new DataPointer(0, (uint)vectorSize, (uint)ValuesSize, (uint)values.Length);
			pointers.Add(p);
			floatSeriesValues.Add(values);
			itemIndex++;
		}
		public void AddSeries(List<float[]> values, int vectorSize)
		{
			foreach (var value in values)
			{
				var floats = value.ToArray();
				AddSeries(floats, vectorSize);
			}
		}
        public void AddSeries(List<Color> colors)
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

        public float[] FlattenedValues
		{
			get
			{
				int len = ValuesSize;
				var result = new float[len];
				int index = 0;
				foreach (var floatSeriesValue in floatSeriesValues)
				{
					foreach (var f in floatSeriesValue)
					{
						result[index++] = f;
					}
				}
				return result;
			}
		}

		public int ValuesSize
		{
			get
			{
				int len = 0;
				foreach (var floatSeriesValue in floatSeriesValues)
				{
					len += floatSeriesValue.Length;
				}
				return len;
			}
		}
		public int ValuesByteSize => ValuesSize * sizeof(float);
		public int PointersSize => pointers.Count;
		public int PointersByteSize => DataPointer.ByteSize * PointersSize;

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
