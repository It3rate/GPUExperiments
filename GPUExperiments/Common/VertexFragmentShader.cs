using OpenTK.Graphics.OpenGL4;

namespace GPUExperiments.Common
{
    public class VertexFragmentShader : ShaderProgram
    {
        public VertexFragmentShader(string vertexShaderPath, string fragmentShaderPath)
        {
            var vertexShaderId = CreateShaderFromPath(vertexShaderPath, ShaderType.VertexShaderArb);
            var fragmentShaderId = CreateShaderFromPath(fragmentShaderPath, ShaderType.FragmentShader);
            CreateProgram(vertexShaderId, fragmentShaderId);
        }

    }
}
