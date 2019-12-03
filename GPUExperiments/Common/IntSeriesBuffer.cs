using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public class IntSeriesBuffer : SeriesBufferBase<int>
    {
	    public override int BufferIndex => (int)BufferSlots.IntSeries;

	    public IntSeriesBuffer()
        {
            Pointers = new List<DataPointer>();
            SeriesValues = new List<int[]>();
        }

        public override void AddSeries(int[] values, int vectorSize)
        {
            DataPointer p = new DataPointer(0, (uint)vectorSize, (uint)Size, (uint)values.Length);
            Pointers.Add(p);
            SeriesValues.Add(values);
        }
        public override void AddSeries(List<int[]> values, int vectorSize)
        {
	        foreach (var value in values)
	        {
		        var ints = value.ToArray();
		        AddSeries(ints, vectorSize);
	        }
        }
        public override void AddSeries(List<Color> colors)
        {
	        var ints = RGBAToInts(colors);
	        AddSeries(ints, 4);
        }

        public override int[] FlattenedValues
        {
            get
            {
                int len = Size;
                var result = new int[len];
                int index = 0;
                foreach (var intSeriesValue in SeriesValues)
                {
                    foreach (var f in intSeriesValue)
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
                foreach (var intSeriesValue in SeriesValues)
                {
                    len += intSeriesValue.Length;
                }
                return len;
            }
        }
        public override int ByteSize => Size * sizeof(int);
		
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
