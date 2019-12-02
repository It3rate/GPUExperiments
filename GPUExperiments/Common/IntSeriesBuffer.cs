using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace GPUExperiments.Common
{
    public class IntSeriesBuffer
    {
        public List<DataPointer> pointers;
        public List<int[]> intSeriesValues;
        private int itemIndex;

        public IntSeriesBuffer()
        {
            pointers = new List<DataPointer>();
            intSeriesValues = new List<int[]>();
            itemIndex = 0;
        }

        public void AddSeries(int[] values, int vectorSize)
        {
            DataPointer p = new DataPointer(0, (uint)vectorSize, (uint)ValuesSize, (uint)values.Length);
            pointers.Add(p);
            intSeriesValues.Add(values);
            itemIndex++;
        }
        public void AddSeries(List<int[]> values, int vectorSize)
        {
	        foreach (var value in values)
	        {
		        var ints = value.ToArray();
		        AddSeries(ints, vectorSize);
	        }
        }
        public void AddSeries(List<Color> colors)
        {
	        var ints = RGBAToInts(colors);
	        AddSeries(ints, 4);
        }

        public int[] FlattenedValues
        {
            get
            {
                int len = ValuesSize;
                var result = new int[len];
                int index = 0;
                foreach (var intSeriesValue in intSeriesValues)
                {
                    foreach (var f in intSeriesValue)
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
                foreach (var intSeriesValue in intSeriesValues)
                {
                    len += intSeriesValue.Length;
                }
                return len;
            }
        }
        public int ValuesByteSize => ValuesSize * sizeof(int);
        public int PointersSize => pointers.Count;
        public int PointersByteSize => DataPointer.ByteSize * PointersSize;
		
        public int[] RGBAToInts(List<Color> colors)
        {
            var result = new int[colors.Count * 4];
            int index = 0;
            foreach (var color in colors)
            {
                result[index++] = color.R;
                result[index++] = color.G;
                result[index++] = color.B;
                result[index++] = color.A;
            }
            return result;
        }
    }
}
