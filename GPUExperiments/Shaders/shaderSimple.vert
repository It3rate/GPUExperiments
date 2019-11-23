#version 330 core

layout(location = 0) in vec2 position;
layout(location = 1) in vec3 color;

//uniform mat4 view;

out vec3 vColor;

void main() 
{
    vColor = color;//vec3(1.0,0.0,0.0);
    //gl_Position = view * vec4(position, 1.0);
    gl_Position = vec4(position, 0.0, 1.0);
}
