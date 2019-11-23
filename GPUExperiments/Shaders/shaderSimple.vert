#version 330 core

layout(location = 0) in vec2 position;
layout(location = 1) in vec4 color;

uniform mat4 view;

out vec4 vColor;

void main() 
{
	float len = length(position)/color.a;
    vColor = vec4(color.rgb, len);
    gl_Position = view * vec4(position, 0.0, 1.0);
    //gl_Position = vec4(position, 0.0, 1.0);
}
