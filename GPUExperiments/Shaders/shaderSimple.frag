#version 330 core

in vec4 vColor;
in vec2 texCoord;
out vec4 color;
uniform sampler2D destTex;

void main()
{
	float edge = smoothstep(0.96, 0.95, vColor.a) - smoothstep(0.72, 0.7, vColor.a);
    color = vColor * edge;
	if(vColor.a < 0.7)
	{
		color = texture(destTex, texCoord) * smoothstep(0.7, 0.6, vColor.a);
	}
	
}