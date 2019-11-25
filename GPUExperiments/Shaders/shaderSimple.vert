#version 330 core

layout(location = 0) in vec2 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoordIn;
uniform sampler2D destTex;

uniform mat4 view;

out vec4 vColor;
out vec2 texCoord;

void main() 
{
	float len = length(position)/color.a;
    vColor = vec4(color.rgb, len);
    texCoord = vec2(position.x * 0.5 + 0.5, 1.0 - position.y * 0.5 + 0.5); //texCoordIn
	gl_Position = view * vec4(position, 0.0, 1.0);
}
