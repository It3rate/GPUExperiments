using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace GPUExperiments.Shaders
{
    public struct VertexPosColor
    {
        public Vector3 Position;
        public Vector3 Color;

        public VertexPosColor(float x, float y, float z, float r, float g, float b)
        {
            Position = new Vector3(x, y, z);
            Color = new Vector3(r, g, b);
        }

        public static int SizeOf => sizeof(float) * 6;
    }
    public struct VertexPosColorTex
    {
        public Vector3 Position;
        public Vector3 Color;
        public Vector2 Texture;

        public VertexPosColorTex(float x, float y, float z, float r, float g, float b, float tx, float ty)
        {
            Position = new Vector3(x, y, z);
            Color = new Vector3(r, g, b);
            Texture = new Vector2(tx, ty);
        }

        public static int SizeOf => sizeof(float) * 8;
    }
}
