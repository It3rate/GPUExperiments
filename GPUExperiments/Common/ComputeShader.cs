using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public class ComputeShader : ShaderBase
    {
        public ComputeShader(string computeShaderPath)
        {
            var computeShaderId = CreateShaderFromPath(computeShaderPath, ShaderType.ComputeShader);
            CreateProgram(computeShaderId);
        }
    }
}
