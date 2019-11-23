#version 330 core

in vec4 vColor;
out vec4 color;

void main()
{
	float edge = smoothstep(0.96, 0.95, vColor.a) - smoothstep(0.42, 0.4, vColor.a);
    color = vColor * edge;
}